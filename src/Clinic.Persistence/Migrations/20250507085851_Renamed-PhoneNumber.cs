using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenamedPhoneNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegistryPhoneNumber",
                table: "Offices",
                newName: "PhoneNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Offices",
                newName: "RegistryPhoneNumber");
        }
    }
}
