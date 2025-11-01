using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HopewellClinicApi.Migrations
{
    /// <inheritdoc />
    public partial class RemoveNurseRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remove all user-role associations for nurse role
            migrationBuilder.Sql(@"
                DELETE FROM AspNetUserRoles 
                WHERE RoleId = '550e8400-e29b-41d4-a716-446655449003'
            ");

            // Remove the nurse role
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655449003"));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441001"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "58e2100f-9073-40f7-a286-52be760428c3", new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(8578), new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(8586) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441003"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "11897c53-24cd-45cf-96bc-d5c06f3ed990", new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(8670), new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(8671) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441004"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "3b9997f0-1d94-4094-87eb-44b0f79a32fc", new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(9116), new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(9116) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(8864), new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(8864) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(8884), new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(8885) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(8913), new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(8913) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440003"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(8917), new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(8917) });

            migrationBuilder.UpdateData(
                table: "services",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440004"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(8920), new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(8920) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441000"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(8977), new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(8978) });

            migrationBuilder.UpdateData(
                table: "staff",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441002"),
                columns: new[] { "created_at", "updated_at" },
                values: new object[] { new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(8984), new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(8984) });

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443101"),
                column: "created_at",
                value: new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(9026));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443102"),
                column: "created_at",
                value: new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(9034));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443103"),
                column: "created_at",
                value: new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(9038));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443201"),
                column: "created_at",
                value: new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(9045));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443202"),
                column: "created_at",
                value: new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(9047));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443203"),
                column: "created_at",
                value: new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(9050));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443301"),
                column: "created_at",
                value: new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(9052));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443302"),
                column: "created_at",
                value: new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(9056));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443303"),
                column: "created_at",
                value: new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(9058));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443401"),
                column: "created_at",
                value: new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(9071));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443402"),
                column: "created_at",
                value: new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(9073));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443403"),
                column: "created_at",
                value: new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(9079));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443501"),
                column: "created_at",
                value: new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(9081));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443502"),
                column: "created_at",
                value: new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(9084));

            migrationBuilder.UpdateData(
                table: "time_slots",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655443503"),
                column: "created_at",
                value: new DateTime(2025, 11, 1, 8, 37, 24, 46, DateTimeKind.Utc).AddTicks(9086));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Re-add the nurse role
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { new Guid("550e8400-e29b-41d4-a716-446655449003"), null, "nurse", "NURSE" });
            
            // Note: User-role associations are not restored automatically
            // They would need to be manually re-added if needed

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
        }
    }
}
