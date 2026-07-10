using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SunmedMedikal.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSunmedFromBrands2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 10, "https://logo.clearbit.com/wtw.com", "WTW" },
                    { 11, "https://logo.clearbit.com/fentex.com", "FENTEX" },
                    { 12, "https://logo.clearbit.com/lpitaliana.com", "LP ITALIANA" },
                    { 13, "https://logo.clearbit.com/isotherm.com", "ISOTHERM" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 13);
        }
    }
}
