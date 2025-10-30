using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HopewellClinicApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorAvailabilityTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "doctor_availability",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    doctor_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    date = table.Column<DateTime>(type: "date", nullable: false),
                    total_slots = table.Column<int>(type: "int", nullable: false),
                    booked_slots = table.Column<int>(type: "int", nullable: false),
                    available_slots = table.Column<int>(type: "int", nullable: false),
                    is_fully_booked = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctor_availability", x => x.id);
                    table.ForeignKey(
                        name: "FK_doctor_availability_staff_doctor_id",
                        column: x => x.doctor_id,
                        principalTable: "staff",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_doctor_availability_date",
                table: "doctor_availability",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "IX_doctor_availability_doctor_id_date",
                table: "doctor_availability",
                columns: new[] { "doctor_id", "date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_doctor_availability_is_fully_booked",
                table: "doctor_availability",
                column: "is_fully_booked");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "doctor_availability");

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
    }
}
