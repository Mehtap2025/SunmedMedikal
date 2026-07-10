using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SunmedMedikal.Migrations
{
    /// <inheritdoc />
    public partial class SeedBrands : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ImageUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/merckgroup.com", "MERCK" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ImageUrl", "Name" },
                values: new object[] { "https://logo.clearbit.com/isolab.de", "ISOLAB" });

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 3, "https://logo.clearbit.com/3m.com", "3M" },
                    { 4, "https://logo.clearbit.com/tekkim.com.tr", "TEKKİM" },
                    { 5, "https://logo.clearbit.com/firatmed.com", "FIRATMED" },
                    { 6, "https://logo.clearbit.com/interscience.com", "INTERSCIENCE" },
                    { 7, "https://logo.clearbit.com/vacusera.com", "VACUSERA" },
                    { 8, "https://logo.clearbit.com/chembio.com", "CHEMBIO" },
                    { 9, "https://logo.clearbit.com/brand.de", "BRAND" },
                    { 10, "/images/Amblem.jpeg", "SUNMED Medikal" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "FullName",
                value: "Mehtap Ş. Önder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ImageUrl", "Name" },
                values: new object[] { "https://via.placeholder.com/200x100/343a40/ffffff?text=Merck", "Merck" });

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ImageUrl", "Name" },
                values: new object[] { "https://via.placeholder.com/200x100/343a40/ffffff?text=Isolab", "Isolab" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "Currency", "Description", "ImageUrl", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Kimyasallar", "TL", "Laboratuvar tipi saf etil alkol.", "https://via.placeholder.com/300x200/343a40/ffffff?text=Etil+Alkol", "Etil Alkol %96", 450.00m },
                    { 2, "Kimyasallar", "TL", "Isıya dayanıklı borosilikat cam beher.", "https://via.placeholder.com/300x200/343a40/ffffff?text=Beher", "Beher Glass 250ml", 120.00m },
                    { 3, "Kimyasallar", "TL", "Hız ayarlı, ısıtıcılı manyetik karıştırıcı cihazı.", "https://via.placeholder.com/300x200/343a40/ffffff?text=Karistirici", "Manyetik Karıştırıcı", 3500.00m }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "FullName",
                value: "Mehtap Önder");
        }
    }
}
