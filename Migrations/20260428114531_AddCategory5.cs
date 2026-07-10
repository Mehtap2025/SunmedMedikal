using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SunmedMedikal.Migrations
{
    /// <inheritdoc />
    public partial class AddCategory5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubSubSubSubCategory",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubSubSubSubCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MainCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubSubCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubSubSubCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubSubSubSubCategories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "SubSubSubSubCategories",
                columns: new[] { "Id", "MainCategory", "Name", "SubCategory", "SubSubCategory", "SubSubSubCategory" },
                values: new object[] { 1, "Cam Malzemeler", "250ml Uzun Form Cam Beherler", "Beherler", "Cam Beherler", "Uzun Form Cam Beherler" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubSubSubSubCategories");

            migrationBuilder.DropColumn(
                name: "SubSubSubSubCategory",
                table: "Products");
        }
    }
}
