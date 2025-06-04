using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactoredKeyForOtps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OtpRecord",
                table: "OtpRecord");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OtpRecord",
                table: "OtpRecord",
                columns: new[] { "Key", "CreatedOnUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OtpRecord",
                table: "OtpRecord");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OtpRecord",
                table: "OtpRecord",
                column: "Key");
        }
    }
}
