using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HopewellClinicApi.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationEnhancements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_PatientId",
                table: "Notifications");

            migrationBuilder.AddColumn<string>(
                name: "ClinicAddress",
                table: "NotificationSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClinicEmail",
                table: "NotificationSettings",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClinicPhone",
                table: "NotificationSettings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PatientId",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "EmailSubject",
                table: "Notifications",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "EmailContent",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SenderId",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderName",
                table: "Notifications",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderRole",
                table: "Notifications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceName",
                table: "Notifications",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ThreadId",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StaffId",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NotificationReplies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ThreadId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SenderRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationReplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotificationReplies_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotificationReplies_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PushSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserRole = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    P256dhKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PushSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PushSubscriptions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441001"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "8dc31d06-6e29-444e-b1e9-cb7511f90714", new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3044), new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3047) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441003"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "f5a1367c-37ec-4b8b-aaf8-8d929659aabf", new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3286), new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3286) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441004"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "e65c15b7-65e6-42b3-8e6c-887606cc7d5e", new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3630), new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3631) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3455), new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3455) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3466), new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3466) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3470), new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3470) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440003"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3473), new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3473) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440004"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3476), new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3476) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3519), new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3519) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3527), new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3528) });

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443101"),
                column: "created_at",
                value: new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3561));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443102"),
                column: "created_at",
                value: new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3567));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443103"),
                column: "created_at",
                value: new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3571));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443201"),
                column: "created_at",
                value: new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3575));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443202"),
                column: "created_at",
                value: new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3577));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443203"),
                column: "created_at",
                value: new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3580));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443301"),
                column: "created_at",
                value: new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3582));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443302"),
                column: "created_at",
                value: new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3588));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443303"),
                column: "created_at",
                value: new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3590));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443401"),
                column: "created_at",
                value: new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3592));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443402"),
                column: "created_at",
                value: new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3595));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443403"),
                column: "created_at",
                value: new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3597));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443501"),
                column: "created_at",
                value: new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3599));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443502"),
                column: "created_at",
                value: new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3602));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443503"),
                column: "created_at",
                value: new DateTime(2025, 10, 30, 22, 23, 39, 642, DateTimeKind.Utc).AddTicks(3607));

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SenderId",
                table: "Notifications",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_StaffId",
                table: "Notifications",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_ThreadId",
                table: "Notifications",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_IsRead",
                table: "Notifications",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SentAt",
                table: "Notifications",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationReplies_NotificationId",
                table: "NotificationReplies",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationReplies_ThreadId",
                table: "NotificationReplies",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationReplies_SenderId",
                table: "NotificationReplies",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_PushSubscriptions_UserId",
                table: "PushSubscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PushSubscriptions_UserRole",
                table: "PushSubscriptions",
                column: "UserRole");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_PatientId",
                table: "Notifications",
                column: "PatientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_staff_StaffId",
                table: "Notifications",
                column: "StaffId",
                principalTable: "staff",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_SenderId",
                table: "Notifications",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_PatientId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_staff_StaffId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_SenderId",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "NotificationReplies");

            migrationBuilder.DropTable(
                name: "PushSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_PushSubscriptions_UserRole",
                table: "PushSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_PushSubscriptions_UserId",
                table: "PushSubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_NotificationReplies_SenderId",
                table: "NotificationReplies");

            migrationBuilder.DropIndex(
                name: "IX_NotificationReplies_ThreadId",
                table: "NotificationReplies");

            migrationBuilder.DropIndex(
                name: "IX_NotificationReplies_NotificationId",
                table: "NotificationReplies");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_SentAt",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_IsRead",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_ThreadId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_StaffId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_SenderId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ClinicAddress",
                table: "NotificationSettings");

            migrationBuilder.DropColumn(
                name: "ClinicEmail",
                table: "NotificationSettings");

            migrationBuilder.DropColumn(
                name: "ClinicPhone",
                table: "NotificationSettings");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "SenderName",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "SenderRole",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ServiceName",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "ThreadId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "Notifications");

            migrationBuilder.AlterColumn<Guid>(
                name: "PatientId",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmailSubject",
                table: "Notifications",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmailContent",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441001"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "bcba3750-e71f-42a1-b846-2ad86d3b37b7", new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3374), new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3386) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441003"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "9d5c3311-effb-4e9b-9b3a-3ac27c90edf6", new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3523), new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3524) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441004"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "ab47f9e4-d2e3-4bf9-a9e7-519d2345bbef", new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(4021), new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(4022) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3746), new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3747) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3761), new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3762) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3765), new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3765) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440003"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3792), new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3793) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440004"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3795), new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3795) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3895), new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3896) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3903), new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3903) });

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443101"),
                column: "created_at",
                value: new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3952));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443102"),
                column: "created_at",
                value: new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3958));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443103"),
                column: "created_at",
                value: new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3962));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443201"),
                column: "created_at",
                value: new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3964));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443202"),
                column: "created_at",
                value: new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3971));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443203"),
                column: "created_at",
                value: new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3975));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443301"),
                column: "created_at",
                value: new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3977));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443302"),
                column: "created_at",
                value: new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3980));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443303"),
                column: "created_at",
                value: new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3982));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443401"),
                column: "created_at",
                value: new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3984));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443402"),
                column: "created_at",
                value: new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3986));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443403"),
                column: "created_at",
                value: new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3989));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443501"),
                column: "created_at",
                value: new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3994));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443502"),
                column: "created_at",
                value: new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3996));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443503"),
                column: "created_at",
                value: new DateTime(2025, 10, 23, 18, 31, 41, 312, DateTimeKind.Utc).AddTicks(3998));

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_PatientId",
                table: "Notifications",
                column: "PatientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
