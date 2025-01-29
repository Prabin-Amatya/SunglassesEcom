using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sunglass_ecom.Migrations
{
    /// <inheritdoc />
    public partial class categoryurl_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Imageurl",
                table: "Category",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imageurl",
                table: "Category");
        }
    }
}
