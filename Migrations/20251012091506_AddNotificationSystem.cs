using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HopewellClinicApi.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentAuditLog_staff_changed_by",
                table: "AppointmentAuditLog");

            migrationBuilder.DropColumn(
                name: "new_values",
                table: "AppointmentAuditLog");

            migrationBuilder.DropColumn(
                name: "old_values",
                table: "AppointmentAuditLog");

            migrationBuilder.RenameColumn(
                name: "changed_by",
                table: "AppointmentAuditLog",
                newName: "performed_by");

            migrationBuilder.RenameColumn(
                name: "changed_at",
                table: "AppointmentAuditLog",
                newName: "performed_at");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentAuditLog_changed_by",
                table: "AppointmentAuditLog",
                newName: "IX_AppointmentAuditLog_performed_by");

            migrationBuilder.AlterColumn<string>(
                name: "action",
                table: "AppointmentAuditLog",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "details",
                table: "AppointmentAuditLog",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "new_status",
                table: "AppointmentAuditLog",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "old_status",
                table: "AppointmentAuditLog",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "reason",
                table: "AppointmentAuditLog",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ScheduledFor = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailSubject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EmailBody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "appointments",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "NotificationSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reminder24hEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Reminder2hEnabled = table.Column<bool>(type: "bit", nullable: false),
                    DefaultEmailTemplate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClinicEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ClinicPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClinicAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSettings", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441001"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "a2deafec-1c6b-4009-9fcd-5548ac00bf88", new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4154), new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4159) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441003"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "ad7bfa19-3ca7-45e5-9a77-96f72c7bdf98", new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4238), new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4238) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441004"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "0d22116a-6aef-4139-8c38-3ba01658057e", new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4723), new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4724) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4459), new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4459) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4474), new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4474) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4500), new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4500) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440003"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4503), new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4503) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440004"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4506), new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4507) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4573), new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4573) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4583), new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4583) });

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443101"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4632));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443102"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4640));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443103"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4643));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443201"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4649));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443202"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4651));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443203"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4655));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443301"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4657));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443302"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4661));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443303"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4664));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443401"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4666));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443402"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4668));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443403"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4684));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443501"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4686));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443502"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4689));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443503"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 9, 15, 3, 677, DateTimeKind.Utc).AddTicks(4692));

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_AppointmentId",
                table: "Notifications",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_PatientId",
                table: "Notifications",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentAuditLog_AspNetUsers_performed_by",
                table: "AppointmentAuditLog",
                column: "performed_by",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentAuditLog_AspNetUsers_performed_by",
                table: "AppointmentAuditLog");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "NotificationSettings");

            migrationBuilder.DropColumn(
                name: "details",
                table: "AppointmentAuditLog");

            migrationBuilder.DropColumn(
                name: "new_status",
                table: "AppointmentAuditLog");

            migrationBuilder.DropColumn(
                name: "old_status",
                table: "AppointmentAuditLog");

            migrationBuilder.DropColumn(
                name: "reason",
                table: "AppointmentAuditLog");

            migrationBuilder.RenameColumn(
                name: "performed_by",
                table: "AppointmentAuditLog",
                newName: "changed_by");

            migrationBuilder.RenameColumn(
                name: "performed_at",
                table: "AppointmentAuditLog",
                newName: "changed_at");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentAuditLog_performed_by",
                table: "AppointmentAuditLog",
                newName: "IX_AppointmentAuditLog_changed_by");

            migrationBuilder.AlterColumn<string>(
                name: "action",
                table: "AppointmentAuditLog",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "new_values",
                table: "AppointmentAuditLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "old_values",
                table: "AppointmentAuditLog",
                type: "nvarchar(max)",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentAuditLog_staff_changed_by",
                table: "AppointmentAuditLog",
                column: "changed_by",
                principalTable: "staff",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
