using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SunmedMedikal.Migrations
{
    /// <inheritdoc />
    public partial class AddSubSubCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubSubCategory",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubSubCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MainCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubSubCategories", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Beherler");

            migrationBuilder.InsertData(
                table: "SubSubCategories",
                columns: new[] { "Id", "MainCategory", "Name", "SubCategory" },
                values: new object[,]
                {
                    { 1, "Cam Malzemeler", "Cam Beherler", "Beherler" },
                    { 2, "Cam Malzemeler", "Plastik Beherler", "Beherler" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubSubCategories");

            migrationBuilder.DropColumn(
                name: "SubSubCategory",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Balon Jojeler");
        }
    }
}
