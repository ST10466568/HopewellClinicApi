using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HopewellClinicApi.Migrations
{
    /// <inheritdoc />
    public partial class ExtendSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441001"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "8f7db8e4-85a5-4331-a180-5dd4006c4496", new DateTime(2025, 8, 21, 20, 33, 10, 858, DateTimeKind.Utc).AddTicks(6906), new DateTime(2025, 8, 21, 20, 33, 10, 858, DateTimeKind.Utc).AddTicks(6907) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441003"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "c3d7e6ae-dae1-4cf7-a0da-7ea95c62dcd7", new DateTime(2025, 8, 21, 20, 33, 10, 859, DateTimeKind.Utc).AddTicks(5142), new DateTime(2025, 8, 21, 20, 33, 10, 859, DateTimeKind.Utc).AddTicks(5144) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 8, 21, 20, 33, 10, 859, DateTimeKind.Utc).AddTicks(9328), new DateTime(2025, 8, 21, 20, 33, 10, 859, DateTimeKind.Utc).AddTicks(9329) });

            migrationBuilder.InsertData(
                table: "services",
                columns: new[] { "id", "created_at", "description", "duration_minutes", "is_active", "name", "updated_at" },
                values: new object[,]
                {
                    { new Guid("550e8400-e29b-41d4-a716-446655440001"), new DateTime(2025, 8, 21, 20, 33, 10, 859, DateTimeKind.Utc).AddTicks(9919), "Health services for infants and children", 30, true, "Pediatrics Consultation", new DateTime(2025, 8, 21, 20, 33, 10, 859, DateTimeKind.Utc).AddTicks(9919) },
                    { new Guid("550e8400-e29b-41d4-a716-446655440002"), new DateTime(2025, 8, 21, 20, 33, 10, 859, DateTimeKind.Utc).AddTicks(9925), "Routine dental examination and cleaning", 45, true, "Dental Checkup", new DateTime(2025, 8, 21, 20, 33, 10, 859, DateTimeKind.Utc).AddTicks(9925) },
                    { new Guid("550e8400-e29b-41d4-a716-446655440003"), new DateTime(2025, 8, 21, 20, 33, 10, 859, DateTimeKind.Utc).AddTicks(9931), "Physical therapy for rehabilitation", 60, true, "Physiotherapy Session", new DateTime(2025, 8, 21, 20, 33, 10, 859, DateTimeKind.Utc).AddTicks(9931) },
                    { new Guid("550e8400-e29b-41d4-a716-446655440004"), new DateTime(2025, 8, 21, 20, 33, 10, 859, DateTimeKind.Utc).AddTicks(9936), "Routine immunizations and boosters", 20, true, "Vaccination", new DateTime(2025, 8, 21, 20, 33, 10, 859, DateTimeKind.Utc).AddTicks(9936) }
                });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 8, 21, 20, 33, 10, 860, DateTimeKind.Utc).AddTicks(436), new DateTime(2025, 8, 21, 20, 33, 10, 860, DateTimeKind.Utc).AddTicks(437) });

            migrationBuilder.InsertData(
                table: "staff",
                columns: new[] { "id", "created_at", "is_active", "staff_number", "updated_at", "user_id" },
                values: new object[] { new Guid("550e8400-e29b-41d4-a716-446655441002"), new DateTime(2025, 8, 21, 20, 33, 10, 860, DateTimeKind.Utc).AddTicks(725), true, "DOC002", new DateTime(2025, 8, 21, 20, 33, 10, 860, DateTimeKind.Utc).AddTicks(725), new Guid("550e8400-e29b-41d4-a716-446655441003") });

            migrationBuilder.InsertData(
                table: "time_slots",
                columns: new[] { "id", "created_at", "day_of_week", "end_time", "is_active", "IsBooked", "start_time" },
                values: new object[,]
                {
                    { new Guid("550e8400-e29b-41d4-a716-446655443101"), new DateTime(2025, 8, 21, 20, 33, 10, 860, DateTimeKind.Utc).AddTicks(1004), 1, new TimeOnly(10, 0, 0), true, false, new TimeOnly(9, 0, 0) },
                    { new Guid("550e8400-e29b-41d4-a716-446655443102"), new DateTime(2025, 8, 21, 20, 33, 10, 860, DateTimeKind.Utc).AddTicks(1593), 1, new TimeOnly(11, 0, 0), true, false, new TimeOnly(10, 0, 0) },
                    { new Guid("550e8400-e29b-41d4-a716-446655443103"), new DateTime(2025, 8, 21, 20, 33, 10, 860, DateTimeKind.Utc).AddTicks(1600), 1, new TimeOnly(12, 0, 0), true, false, new TimeOnly(11, 0, 0) },
                    { new Guid("550e8400-e29b-41d4-a716-446655443201"), new DateTime(2025, 8, 21, 20, 33, 10, 860, DateTimeKind.Utc).AddTicks(1605), 2, new TimeOnly(10, 0, 0), true, false, new TimeOnly(9, 0, 0) },
                    { new Guid("550e8400-e29b-41d4-a716-446655443202"), new DateTime(2025, 8, 21, 20, 33, 10, 860, DateTimeKind.Utc).AddTicks(1611), 2, new TimeOnly(11, 0, 0), true, false, new TimeOnly(10, 0, 0) },
                    { new Guid("550e8400-e29b-41d4-a716-446655443203"), new DateTime(2025, 8, 21, 20, 33, 10, 860, DateTimeKind.Utc).AddTicks(1615), 2, new TimeOnly(12, 0, 0), true, false, new TimeOnly(11, 0, 0) },
                    { new Guid("550e8400-e29b-41d4-a716-446655443301"), new DateTime(2025, 8, 21, 20, 33, 10, 860, DateTimeKind.Utc).AddTicks(1620), 3, new TimeOnly(10, 0, 0), true, false, new TimeOnly(9, 0, 0) },
                    { new Guid("550e8400-e29b-41d4-a716-446655443302"), new DateTime(2025, 8, 21, 20, 33, 10, 860, DateTimeKind.Utc).AddTicks(1626), 3, new TimeOnly(11, 0, 0), true, false, new TimeOnly(10, 0, 0) },
                    { new Guid("550e8400-e29b-41d4-a716-446655443303"), new DateTime(2025, 8, 21, 20, 33, 10, 860, DateTimeKind.Utc).AddTicks(1631), 3, new TimeOnly(12, 0, 0), true, false, new TimeOnly(11, 0, 0) },
                    { new Guid("550e8400-e29b-41d4-a716-446655443401"), new DateTime(2025, 8, 21, 20, 33, 10, 860, DateTimeKind.Utc).AddTicks(1635), 4, new TimeOnly(10, 0, 0), true, false, new TimeOnly(9, 0, 0) },
                    { new Guid("550e8400-e29b-41d4-a716-446655443402"), new DateTime(2025, 8, 21, 20, 33, 10, 860, DateTimeKind.Utc).AddTicks(1640), 4, new TimeOnly(11, 0, 0), true, false, new TimeOnly(10, 0, 0) },
                    { new Guid("550e8400-e29b-41d4-a716-446655443403"), new DateTime(2025, 8, 21, 20, 33, 10, 860, DateTimeKind.Utc).AddTicks(1644), 4, new TimeOnly(12, 0, 0), true, false, new TimeOnly(11, 0, 0) },
                    { new Guid("550e8400-e29b-41d4-a716-446655443501"), new DateTime(2025, 8, 21, 20, 33, 10, 860, DateTimeKind.Utc).AddTicks(1650), 5, new TimeOnly(10, 0, 0), true, false, new TimeOnly(9, 0, 0) },
                    { new Guid("550e8400-e29b-41d4-a716-446655443502"), new DateTime(2025, 8, 21, 20, 33, 10, 860, DateTimeKind.Utc).AddTicks(1718), 5, new TimeOnly(11, 0, 0), true, false, new TimeOnly(10, 0, 0) },
                    { new Guid("550e8400-e29b-41d4-a716-446655443503"), new DateTime(2025, 8, 21, 20, 33, 10, 860, DateTimeKind.Utc).AddTicks(1723), 5, new TimeOnly(12, 0, 0), true, false, new TimeOnly(11, 0, 0) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"));

            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"));

            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440003"));

            migrationBuilder.DeleteData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440004"));

            migrationBuilder.DeleteData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441002"));

            migrationBuilder.DeleteData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443101"));

            migrationBuilder.DeleteData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443102"));

            migrationBuilder.DeleteData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443103"));

            migrationBuilder.DeleteData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443201"));

            migrationBuilder.DeleteData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443202"));

            migrationBuilder.DeleteData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443203"));

            migrationBuilder.DeleteData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443301"));

            migrationBuilder.DeleteData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443302"));

            migrationBuilder.DeleteData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443303"));

            migrationBuilder.DeleteData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443401"));

            migrationBuilder.DeleteData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443402"));

            migrationBuilder.DeleteData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443403"));

            migrationBuilder.DeleteData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443501"));

            migrationBuilder.DeleteData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443502"));

            migrationBuilder.DeleteData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443503"));

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
    }
}
