using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SunmedMedikal.Migrations
{
    /// <inheritdoc />
    public partial class Add4thLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "SubSubSubCategory",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubSubSubCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MainCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubSubCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubSubSubCategories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SubSubSubCategories",
                columns: new[] { "Id", "MainCategory", "Name", "SubCategory", "SubSubCategory" },
                values: new object[] { 1, "Cam Malzemeler", "Uzun Form Cam Beherler", "Beherler", "Cam Beherler" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubSubSubCategories");

            migrationBuilder.DropColumn(
                name: "SubSubSubCategory",
                table: "Products");

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { 3, "/images/brands/3MLogo.png", "3M" },
                    { 4, "/images/brands/tekkimlogo.png", "TEKKİM" },
                    { 5, "/images/brands/firatmedlogo.png", "FIRATMED" },
                    { 6, "/images/brands/intersciencelogo.png", "INTERSCIENCE" },
                    { 7, "/images/brands/Vacuseralogo.png", "VACUSERA" },
                    { 8, "/images/brands/chembiologo.png", "CHEMBIO" },
                    { 9, "/images/brands/Brandlogo.png", "BRAND" },
                    { 10, "/images/brands/wtwlogo.png", "WTW" },
                    { 11, "/images/brands/fentexlogo.png", "FENTEX" },
                    { 12, "/images/brands/LPitalianalogo.png", "LP ITALIANA" },
                    { 13, "/images/brands/isothermlogo.png", "ISOTHERM" }
                });
        }
    }
}
