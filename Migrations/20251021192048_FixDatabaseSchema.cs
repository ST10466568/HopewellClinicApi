using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HopewellClinicApi.Migrations
{
    /// <inheritdoc />
    public partial class FixDatabaseSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyContact",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmergencyPhone",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441001"),
                columns: new[] { "Address", "ConcurrencyStamp", "CreatedAt", "DateOfBirth", "EmergencyContact", "EmergencyPhone", "UpdatedAt" },
                values: new object[] { null, "7bd3f4f5-07f6-4359-96b1-485229f5605a", new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(7928), null, null, null, new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(7935) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441003"),
                columns: new[] { "Address", "ConcurrencyStamp", "CreatedAt", "DateOfBirth", "EmergencyContact", "EmergencyPhone", "UpdatedAt" },
                values: new object[] { null, "772b870f-2172-49dc-8da6-93e1e1d4a0f0", new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8093), null, null, null, new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8093) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441004"),
                columns: new[] { "Address", "ConcurrencyStamp", "CreatedAt", "DateOfBirth", "EmergencyContact", "EmergencyPhone", "UpdatedAt" },
                values: new object[] { null, "ff347288-f936-46f0-9a3e-27a6b0509bbd", new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8914), null, null, null, new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8914) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8420), new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8421) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8452), new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8453) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8458), new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8458) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440003"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8462), new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8462) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440004"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8468), new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8469) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8609), new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8609) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8614), new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8614) });

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443101"),
                column: "created_at",
                value: new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8690));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443102"),
                column: "created_at",
                value: new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8720));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443103"),
                column: "created_at",
                value: new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8724));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443201"),
                column: "created_at",
                value: new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8802));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443202"),
                column: "created_at",
                value: new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8805));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443203"),
                column: "created_at",
                value: new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8809));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443301"),
                column: "created_at",
                value: new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8820));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443302"),
                column: "created_at",
                value: new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8823));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443303"),
                column: "created_at",
                value: new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8828));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443401"),
                column: "created_at",
                value: new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8832));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443402"),
                column: "created_at",
                value: new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8836));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443403"),
                column: "created_at",
                value: new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8841));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443501"),
                column: "created_at",
                value: new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8853));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443502"),
                column: "created_at",
                value: new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8858));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443503"),
                column: "created_at",
                value: new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8863));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmergencyContact",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmergencyPhone",
                table: "AspNetUsers");

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
    }
}
