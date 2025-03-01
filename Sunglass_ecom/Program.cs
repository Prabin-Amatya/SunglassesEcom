using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sunglass_ecom.Data;
using Sunglass_ecom.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Sunglass_ecom.Utils;
using Recommendaton_Modal;
using System;
using System.Text.Json.Serialization;
using Sunglass_ecom.Interfaces;
using Sunglass_ecom.Repositoriess;
using BC = BCrypt.Net.BCrypt;
using Sunglass_ecom.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var provider = builder.Services.BuildServiceProvider();
var config = provider.GetService<IConfiguration>();

builder.Services.AddDbContext<EcommerceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("conn")));

builder.Services.AddControllers()
    .AddJsonOptions(options => {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddTransient<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<KhaltiService>();
builder.Services.AddSingleton(o => new BagOfWordsModel());
builder.Services.AddSingleton(ServiceProvider =>
{
    var model = ServiceProvider.GetRequiredService<BagOfWordsModel>();
    var scopeFactory = ServiceProvider.GetRequiredService<IServiceScopeFactory>();
    return new Vectors(model, scopeFactory);
});
/*{

    Vectors vectors = new Vectors();
    var Task =  vectors.initializeAsync(ServiceProvider);
    Task.Wait();
    return vectors;
});*/

builder.Services.AddAuthentication(cfg => {
    cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    cfg.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {
    x.RequireHttpsMetadata = false;
    x.SaveToken = false;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8
            .GetBytes(builder.Configuration["ApplicationSettings:JWT_Secret"])
        ),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});


builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWTToken_Auth_API",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .WithHeaders("Authorization", "Content-Type");
                      });
});


var app = builder.Build();

app.UseStaticFiles();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var _dbcontext = services.GetRequiredService<EcommerceDbContext>();
    var admin = _dbcontext.Users.FirstOrDefault(p=>p.Email == "admin@gmail.com");
    if (admin == null)
    {
        User user = new User();
        user.Username = "admin@gmail.com";
        user.Role = "Admin";
        user.Email = "admin@gmail.com";
        user.Password = BC.HashPassword("admin@123", workFactor: 10); ;
        user.FirstName = "admin";
        user.LastName = "admin";
        user.City = "Address";
        user.Streets = "Street";
        user.Zones = "Zone";
        user.PhoneNumber = "99797979";
        _dbcontext.Users.Add(user);
        _dbcontext.SaveChanges();
    }
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors(MyAllowSpecificOrigins);
if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
};
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // Ensure this is correctly set up
});



app.MapControllers();

app.Run();
