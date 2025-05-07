using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SetPhoneUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Offices_PhoneNumber",
                table: "Offices",
                column: "PhoneNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Offices_PhoneNumber",
                table: "Offices");
        }
    }
}
