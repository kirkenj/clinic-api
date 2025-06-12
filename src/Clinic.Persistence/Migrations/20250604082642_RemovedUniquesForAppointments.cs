using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUniquesForAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appointment_DateTimeStart_DoctorId",
                table: "Appointment");

            migrationBuilder.DropIndex(
                name: "IX_Appointment_DateTimeStart_PatientId",
                table: "Appointment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Appointment_DateTimeStart_DoctorId",
                table: "Appointment",
                columns: new[] { "DateTimeStart", "DoctorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_DateTimeStart_PatientId",
                table: "Appointment",
                columns: new[] { "DateTimeStart", "PatientId" },
                unique: true);
        }
    }
}
