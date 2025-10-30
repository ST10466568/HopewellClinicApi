using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HopewellClinicApi.Migrations
{
    /// <inheritdoc />
    public partial class AddShiftScheduleValidationToDoctorAvailability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_on_duty",
                table: "doctor_availability",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "unavailability_reason",
                table: "doctor_availability",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441001"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "d5ce75ad-c4f7-4bae-a8b5-51170db2470f", new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(1878), new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(1881) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441003"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "fe35721c-eff4-4408-ae1d-18e32527cbc4", new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(1956), new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(1957) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441004"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "b10f59ee-1b95-418f-9a6a-e8d229137988", new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2527), new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2528) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2241), new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2242) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2255), new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2256) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2260), new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2261) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440003"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2266), new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2267) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440004"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2280), new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2281) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2336), new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2336) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2345), new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2345) });

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443101"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2401));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443102"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2410));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443103"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2414));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443201"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2417));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443202"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2420));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443203"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2426));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443301"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2429));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443302"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2432));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443303"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2434));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443401"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2438));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443402"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2441));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443403"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2454));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443501"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2457));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443502"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2464));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443503"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 17, 25, 52, 265, DateTimeKind.Utc).AddTicks(2469));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_on_duty",
                table: "doctor_availability");

            migrationBuilder.DropColumn(
                name: "unavailability_reason",
                table: "doctor_availability");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441001"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "ed498cf2-5f5c-48b7-b5a6-0d5c67d031e4", new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(6758), new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(6769) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441003"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "d5d8e1d0-12c0-4a97-9e43-46aa19cd8583", new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(6947), new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(6947) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441004"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "5e6ac159-918a-4acb-a76e-7f65db3413e4", new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7726), new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7727) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7350), new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7350) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7387), new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7388) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7391), new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7391) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440003"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7398), new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7398) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440004"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7401), new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7401) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7471), new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7471) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7478), new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7478) });

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443101"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7629));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443102"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7634));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443103"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7645));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443201"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7651));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443202"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7654));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443203"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7657));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443301"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7659));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443302"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7662));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443303"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7667));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443401"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7669));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443402"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7672));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443403"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7674));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443501"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7678));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443502"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7680));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443503"),
                column: "created_at",
                value: new DateTime(2025, 10, 15, 16, 29, 37, 542, DateTimeKind.Utc).AddTicks(7682));
        }
    }
}
