using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HopewellClinicApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAppointmentAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorSchedules");

            migrationBuilder.CreateTable(
                name: "AppointmentAuditLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    appointment_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    action = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    changed_by = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    changed_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    old_values = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    new_values = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentAuditLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentAuditLog_appointments_appointment_id",
                        column: x => x.appointment_id,
                        principalTable: "appointments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentAuditLog_staff_changed_by",
                        column: x => x.changed_by,
                        principalTable: "staff",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441001"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "43a18c73-ac88-4c6f-9092-e061522e99cf", new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(5420), new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(5426) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441003"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "26d71edf-83a9-4c87-a093-77f17503ca78", new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(5686), new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(5691) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441004"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "b5b9ccc6-8763-4815-b992-e365c626283a", new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6556), new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6557) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6103), new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6114) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6154), new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6154) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6158), new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6158) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440003"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6165), new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6165) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440004"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6237), new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6237) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6326), new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6331) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6347), new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6348) });

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443101"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6407));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443102"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6423));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443103"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6433));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443201"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6444));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443202"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6447));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443203"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6458));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443301"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6461));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443302"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6464));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443303"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6471));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443401"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6476));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443402"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6478));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443403"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6483));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443501"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6485));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443502"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6491));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443503"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 19, 59, 6, 603, DateTimeKind.Utc).AddTicks(6494));

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentAuditLog_appointment_id",
                table: "AppointmentAuditLog",
                column: "appointment_id");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentAuditLog_changed_by",
                table: "AppointmentAuditLog",
                column: "changed_by");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentAuditLog");

            migrationBuilder.CreateTable(
                name: "DoctorSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BreakEnd = table.Column<TimeSpan>(type: "time", nullable: true),
                    BreakStart = table.Column<TimeSpan>(type: "time", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    DayOfWeek = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ShiftEnd = table.Column<TimeSpan>(type: "time", nullable: false),
                    ShiftStart = table.Column<TimeSpan>(type: "time", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorSchedules_staff_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "staff",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441001"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "caedaf63-1b6b-44f0-9fdc-19d4cd24b445", new DateTime(2025, 10, 11, 11, 31, 55, 832, DateTimeKind.Utc).AddTicks(9174), new DateTime(2025, 10, 11, 11, 31, 55, 832, DateTimeKind.Utc).AddTicks(9183) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441003"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "c390efd6-1751-4cf0-90c9-c46f94c15042", new DateTime(2025, 10, 11, 11, 31, 55, 832, DateTimeKind.Utc).AddTicks(9597), new DateTime(2025, 10, 11, 11, 31, 55, 832, DateTimeKind.Utc).AddTicks(9598) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441004"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "097a6434-b3b0-406c-895b-7cb1797b2ffe", new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(768), new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(769) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(86), new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(88) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(143), new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(143) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(154), new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(154) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440003"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(163), new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(163) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440004"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(170), new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(170) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(407), new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(408) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(432), new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(432) });

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443101"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(545));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443102"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(576));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443103"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(582));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443201"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(589));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443202"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(593));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443203"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(602));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443301"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(618));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443302"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(629));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443303"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(636));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443401"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(639));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443402"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(647));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443403"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(674));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443501"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(689));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443502"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(694));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443503"),
                column: "created_at",
                value: new DateTime(2025, 10, 11, 11, 31, 55, 833, DateTimeKind.Utc).AddTicks(703));

            migrationBuilder.CreateIndex(
                name: "IX_DoctorSchedules_DoctorId_DayOfWeek_Date",
                table: "DoctorSchedules",
                columns: new[] { "DoctorId", "DayOfWeek", "Date" },
                unique: true);
        }
    }
}
