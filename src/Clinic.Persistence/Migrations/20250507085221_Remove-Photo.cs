using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovePhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Offices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
