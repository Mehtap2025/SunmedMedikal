using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SunmedMedikal.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBrandLogosLocal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "/images/brands/MERCKLOGO.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "/images/brands/isolablogo.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "/images/brands/3MLogo.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "/images/brands/tekkimlogo.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "/images/brands/firatmedlogo.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImageUrl",
                value: "/images/brands/intersciencelogo.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImageUrl",
                value: "/images/brands/Vacuseralogo.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImageUrl",
                value: "/images/brands/chembiologo.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 9,
                column: "ImageUrl",
                value: "/images/brands/Brandlogo.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 10,
                column: "ImageUrl",
                value: "/images/brands/wtwlogo.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 11,
                column: "ImageUrl",
                value: "/images/brands/fentexlogo.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 12,
                column: "ImageUrl",
                value: "/images/brands/LPitalianalogo.png");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 13,
                column: "ImageUrl",
                value: "/images/brands/isothermlogo.png");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "https://logo.clearbit.com/merckgroup.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "https://logo.clearbit.com/isolab.de");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "https://logo.clearbit.com/3m.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImageUrl",
                value: "https://logo.clearbit.com/tekkim.com.tr");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImageUrl",
                value: "https://logo.clearbit.com/firatmed.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImageUrl",
                value: "https://logo.clearbit.com/interscience.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImageUrl",
                value: "https://logo.clearbit.com/vacusera.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImageUrl",
                value: "https://logo.clearbit.com/chembio.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 9,
                column: "ImageUrl",
                value: "https://logo.clearbit.com/brand.de");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 10,
                column: "ImageUrl",
                value: "https://logo.clearbit.com/wtw.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 11,
                column: "ImageUrl",
                value: "https://logo.clearbit.com/fentex.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 12,
                column: "ImageUrl",
                value: "https://logo.clearbit.com/lpitaliana.com");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Id",
                keyValue: 13,
                column: "ImageUrl",
                value: "https://logo.clearbit.com/isotherm.com");
        }
    }
}
