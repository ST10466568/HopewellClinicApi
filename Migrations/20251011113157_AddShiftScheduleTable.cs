using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HopewellClinicApi.Migrations
{
    /// <inheritdoc />
    public partial class AddShiftScheduleTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShiftSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftSchedules", x => x.Id);
                    table.CheckConstraint("CK_ShiftSchedules_DayOfWeek", "DayOfWeek IN ('Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday')");
                    table.CheckConstraint("CK_ShiftSchedules_TimeRange", "EndTime > StartTime");
                    table.ForeignKey(
                        name: "FK_ShiftSchedules_staff_DoctorId",
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
                name: "IX_ShiftSchedules_DoctorId_DayOfWeek",
                table: "ShiftSchedules",
                columns: new[] { "DoctorId", "DayOfWeek" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShiftSchedules");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441001"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "26c44625-2372-4692-a0cd-db78a2064958", new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(3012), new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(3017) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441003"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "b38d2416-3fa2-48fc-9026-5cefc994aeed", new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(3114), new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(3115) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441004"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "8f08722f-b053-4255-a4f7-54ead42af25b", new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(4527), new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(4528) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(3654), new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(3655) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(3676), new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(3677) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(3687), new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(3688) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440003"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(3697), new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(3699) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440004"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(3708), new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(3710) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(3906), new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(3908) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(3922), new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(3923) });

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443101"),
                column: "created_at",
                value: new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(4105));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443102"),
                column: "created_at",
                value: new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(4122));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443103"),
                column: "created_at",
                value: new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(4131));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443201"),
                column: "created_at",
                value: new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(4140));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443202"),
                column: "created_at",
                value: new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(4148));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443203"),
                column: "created_at",
                value: new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(4157));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443301"),
                column: "created_at",
                value: new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(4175));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443302"),
                column: "created_at",
                value: new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(4183));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443303"),
                column: "created_at",
                value: new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(4191));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443401"),
                column: "created_at",
                value: new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(4200));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443402"),
                column: "created_at",
                value: new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(4208));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443403"),
                column: "created_at",
                value: new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(4233));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443501"),
                column: "created_at",
                value: new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(4242));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443502"),
                column: "created_at",
                value: new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(4251));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443503"),
                column: "created_at",
                value: new DateTime(2025, 10, 7, 19, 49, 10, 148, DateTimeKind.Utc).AddTicks(4423));
        }
    }
}
