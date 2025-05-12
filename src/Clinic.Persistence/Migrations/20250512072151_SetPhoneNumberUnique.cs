using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SetPhoneNumberUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Doctors_PhoneNumber",
                table: "Doctors",
                column: "PhoneNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Doctors_PhoneNumber",
                table: "Doctors");
        }
    }
}
