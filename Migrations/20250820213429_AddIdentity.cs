using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HopewellClinicApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441001"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "e923c684-fb81-4d83-bdd8-b0ecb4ca171d", new DateTime(2025, 8, 20, 21, 34, 28, 314, DateTimeKind.Utc).AddTicks(3508), new DateTime(2025, 8, 20, 21, 34, 28, 314, DateTimeKind.Utc).AddTicks(3518) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441003"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "808bacb8-245b-4df9-ab34-4ebe8262384f", new DateTime(2025, 8, 20, 21, 34, 28, 315, DateTimeKind.Utc).AddTicks(7918), new DateTime(2025, 8, 20, 21, 34, 28, 315, DateTimeKind.Utc).AddTicks(7923) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 8, 20, 21, 34, 28, 316, DateTimeKind.Utc).AddTicks(3820), new DateTime(2025, 8, 20, 21, 34, 28, 316, DateTimeKind.Utc).AddTicks(3823) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 8, 20, 21, 34, 28, 316, DateTimeKind.Utc).AddTicks(6238), new DateTime(2025, 8, 20, 21, 34, 28, 316, DateTimeKind.Utc).AddTicks(6240) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441001"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "66f5dba2-7e02-4b03-b0b9-8e92220299ca", new DateTime(2025, 8, 20, 21, 11, 36, 836, DateTimeKind.Utc).AddTicks(2101), new DateTime(2025, 8, 20, 21, 11, 36, 836, DateTimeKind.Utc).AddTicks(2108) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441003"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "384fd077-d052-4aef-9af6-d5f97f130a17", new DateTime(2025, 8, 20, 21, 11, 36, 837, DateTimeKind.Utc).AddTicks(5723), new DateTime(2025, 8, 20, 21, 11, 36, 837, DateTimeKind.Utc).AddTicks(5728) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 8, 20, 21, 11, 36, 838, DateTimeKind.Utc).AddTicks(2882), new DateTime(2025, 8, 20, 21, 11, 36, 838, DateTimeKind.Utc).AddTicks(2885) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 8, 20, 21, 11, 36, 838, DateTimeKind.Utc).AddTicks(4986), new DateTime(2025, 8, 20, 21, 11, 36, 838, DateTimeKind.Utc).AddTicks(4986) });
        }
    }
}
