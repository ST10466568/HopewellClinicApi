using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HopewellClinicApi.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetTokens_AspNetUsers_UserId",
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

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_ExpiresAt",
                table: "PasswordResetTokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_Token",
                table: "PasswordResetTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetTokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441001"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "7bd3f4f5-07f6-4359-96b1-485229f5605a", new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(7928), new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(7935) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441003"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "772b870f-2172-49dc-8da6-93e1e1d4a0f0", new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8093), new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8093) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655441004"),
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "UpdatedAt" },
                values: new object[] { "ff347288-f936-46f0-9a3e-27a6b0509bbd", new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8914), new DateTime(2025, 10, 21, 19, 20, 46, 744, DateTimeKind.Utc).AddTicks(8914) });

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
    }
}
