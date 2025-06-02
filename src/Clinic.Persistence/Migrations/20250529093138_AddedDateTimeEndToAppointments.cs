using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedDateTimeEndToAppointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "Appointments",
                newName: "DateTimeStart");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_DateTime_PatientId",
                table: "Appointments",
                newName: "IX_Appointments_DateTimeStart_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_DateTime_DoctorId",
                table: "Appointments",
                newName: "IX_Appointments_DateTimeStart_DoctorId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTimeEnd",
                table: "Appointments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddCheckConstraint(
                name: "CK_DateStart_DateEnd",
                table: "Appointments",
                sql: "'DateTimeEnd' > 'DateTimeStart'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_DateStart_DateEnd",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "DateTimeEnd",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "DateTimeStart",
                table: "Appointments",
                newName: "DateTime");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_DateTimeStart_PatientId",
                table: "Appointments",
                newName: "IX_Appointments_DateTime_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_DateTimeStart_DoctorId",
                table: "Appointments",
                newName: "IX_Appointments_DateTime_DoctorId");
        }
    }
}
