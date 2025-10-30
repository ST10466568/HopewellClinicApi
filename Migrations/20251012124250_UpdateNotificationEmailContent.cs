using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HopewellClinicApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNotificationEmailContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "DefaultEmailTemplate",
                table: "NotificationSettings");

            migrationBuilder.RenameColumn(
                name: "Reminder2hEnabled",
                table: "NotificationSettings",
                newName: "TestResultAlerts");

            migrationBuilder.RenameColumn(
                name: "Reminder24hEnabled",
                table: "NotificationSettings",
                newName: "SmsNotifications");

            migrationBuilder.RenameColumn(
                name: "EmailBody",
                table: "Notifications",
                newName: "EmailContent");

            migrationBuilder.AddColumn<bool>(
                name: "AppointmentConfirmations",
                table: "NotificationSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AutoReminder24h",
                table: "NotificationSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AutoReminder2h",
                table: "NotificationSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EmailNotifications",
                table: "NotificationSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "InsuranceReminders",
                table: "NotificationSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PrescriptionAlerts",
                table: "NotificationSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Notifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Notifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441001"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "50c4a9e7-0663-4844-a602-d714f7274634", new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(7681), new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(7692) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441003"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "e716dfdf-c80c-47e1-a622-f69666d1dd7b", new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(7882), new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(7882) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441004"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "17e8abda-5a89-4797-aebe-1441fbea8a40", new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(9226), new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(9226) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(8470), new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(8471) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(8523), new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(8523) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(8530), new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(8531) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440003"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(8557), new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(8557) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440004"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(8567), new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(8568) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(8754), new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(8754) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(8780), new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(8781) });

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443101"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(8929));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443102"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(8966));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443103"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(8982));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443201"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(8996));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443202"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(9022));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443203"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(9044));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443301"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(9051));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443302"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(9060));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443303"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(9066));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443401"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(9075));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443402"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(9083));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443403"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(9119));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443501"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(9131));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443502"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(9140));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443503"),
                column: "created_at",
                value: new DateTime(2025, 10, 12, 12, 42, 48, 912, DateTimeKind.Utc).AddTicks(9149));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppointmentConfirmations",
                table: "NotificationSettings");

            migrationBuilder.DropColumn(
                name: "AutoReminder24h",
                table: "NotificationSettings");

            migrationBuilder.DropColumn(
                name: "AutoReminder2h",
                table: "NotificationSettings");

            migrationBuilder.DropColumn(
                name: "EmailNotifications",
                table: "NotificationSettings");

            migrationBuilder.DropColumn(
                name: "InsuranceReminders",
                table: "NotificationSettings");

            migrationBuilder.DropColumn(
                name: "PrescriptionAlerts",
                table: "NotificationSettings");

            migrationBuilder.RenameColumn(
                name: "TestResultAlerts",
                table: "NotificationSettings",
                newName: "Reminder2hEnabled");

            migrationBuilder.RenameColumn(
                name: "SmsNotifications",
                table: "NotificationSettings",
                newName: "Reminder24hEnabled");

            migrationBuilder.RenameColumn(
                name: "EmailContent",
                table: "Notifications",
                newName: "EmailBody");

            migrationBuilder.AddColumn<string>(
                name: "ClinicAddress",
                table: "NotificationSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClinicEmail",
                table: "NotificationSettings",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClinicPhone",
                table: "NotificationSettings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DefaultEmailTemplate",
                table: "NotificationSettings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Notifications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Notifications",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

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
        }
    }
}
