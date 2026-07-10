using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SunmedMedikal.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSunmedFromBrands : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 10);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "ImageUrl", "Name" },
                values: new object[] { 10, "/images/Amblem.jpeg", "SUNMED Medikal" });
        }
    }
}
