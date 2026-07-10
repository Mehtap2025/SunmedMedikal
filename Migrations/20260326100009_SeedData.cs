using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SunmedMedikal.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "LogoUrl", "Name" },
                values: new object[,]
                {
                    { 1, "https://via.placeholder.com/200x100/343a40/ffffff?text=Merck", "Merck" },
                    { 2, "https://via.placeholder.com/200x100/343a40/ffffff?text=Isolab", "Isolab" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Laboratuvar tipi saf etil alkol.", "https://via.placeholder.com/300x200/343a40/ffffff?text=Etil+Alkol", "Etil Alkol %96", 450.00m },
                    { 2, "Isıya dayanıklı borosilikat cam beher.", "https://via.placeholder.com/300x200/343a40/ffffff?text=Beher", "Beher Glass 250ml", 120.00m },
                    { 3, "Hız ayarlı, ısıtıcılı manyetik karıştırıcı cihazı.", "https://via.placeholder.com/300x200/343a40/ffffff?text=Karistirici", "Manyetik Karıştırıcı", 3500.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
