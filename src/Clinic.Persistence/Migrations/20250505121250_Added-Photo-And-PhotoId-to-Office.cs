using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedPhotoAndPhotoIdtoOffice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Offices");

            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "Offices",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhotoId",
                table: "Offices",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Offices");

            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Offices",
                type: "text",
                nullable: true);
        }
    }
}
