using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HopewellClinicApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentity2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441001"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "2012b59e-3df5-442e-a72f-ef7e34c3dbf6", new DateTime(2025, 8, 20, 21, 39, 7, 818, DateTimeKind.Utc).AddTicks(2180), new DateTime(2025, 8, 20, 21, 39, 7, 818, DateTimeKind.Utc).AddTicks(2188) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441003"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "69cb8011-894f-4010-89e6-55cd9e8fd596", new DateTime(2025, 8, 20, 21, 39, 7, 819, DateTimeKind.Utc).AddTicks(5595), new DateTime(2025, 8, 20, 21, 39, 7, 819, DateTimeKind.Utc).AddTicks(5600) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 8, 20, 21, 39, 7, 820, DateTimeKind.Utc).AddTicks(1702), new DateTime(2025, 8, 20, 21, 39, 7, 820, DateTimeKind.Utc).AddTicks(1704) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 8, 20, 21, 39, 7, 820, DateTimeKind.Utc).AddTicks(4051), new DateTime(2025, 8, 20, 21, 39, 7, 820, DateTimeKind.Utc).AddTicks(4053) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
