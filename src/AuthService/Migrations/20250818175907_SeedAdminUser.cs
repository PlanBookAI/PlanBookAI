using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "auth",
                table: "roles",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "updated_at", "created_at" },
                values: new object[] { new DateTime(2025, 8, 18, 17, 59, 6, 426, DateTimeKind.Utc).AddTicks(6143), new DateTime(2025, 8, 18, 17, 59, 6, 426, DateTimeKind.Utc).AddTicks(6139) });

            migrationBuilder.UpdateData(
                schema: "auth",
                table: "roles",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "updated_at", "created_at" },
                values: new object[] { new DateTime(2025, 8, 18, 17, 59, 6, 426, DateTimeKind.Utc).AddTicks(6972), new DateTime(2025, 8, 18, 17, 59, 6, 426, DateTimeKind.Utc).AddTicks(6971) });

            migrationBuilder.UpdateData(
                schema: "auth",
                table: "roles",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "updated_at", "created_at" },
                values: new object[] { new DateTime(2025, 8, 18, 17, 59, 6, 426, DateTimeKind.Utc).AddTicks(6975), new DateTime(2025, 8, 18, 17, 59, 6, 426, DateTimeKind.Utc).AddTicks(6975) });

            migrationBuilder.UpdateData(
                schema: "auth",
                table: "roles",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "updated_at", "created_at" },
                values: new object[] { new DateTime(2025, 8, 18, 17, 59, 6, 426, DateTimeKind.Utc).AddTicks(6977), new DateTime(2025, 8, 18, 17, 59, 6, 426, DateTimeKind.Utc).AddTicks(6976) });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "users",
                columns: new[] { "id", "email", "is_active", "last_login", "password_hash", "updated_at", "created_at", "role_id" },
                values: new object[] { new Guid("550e8400-e29b-41d4-a716-446655440001"), "admin@planbookai.com", true, null, "$2a$11$Vscop1CEaxgYeLkKQ8.zPebUcHwwmeF9fhEDNbF3uDostDsQr3VoC", new DateTime(2025, 8, 18, 17, 59, 6, 681, DateTimeKind.Utc).AddTicks(6172), new DateTime(2025, 8, 18, 17, 59, 6, 681, DateTimeKind.Utc).AddTicks(6166), 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "auth",
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("550e8400-e29b-41d4-a716-446655440001"));

            migrationBuilder.UpdateData(
                schema: "auth",
                table: "roles",
                keyColumn: "id",
                keyValue: 1,
                columns: new[] { "updated_at", "created_at" },
                values: new object[] { new DateTime(2025, 8, 18, 17, 57, 4, 879, DateTimeKind.Utc).AddTicks(4243), new DateTime(2025, 8, 18, 17, 57, 4, 879, DateTimeKind.Utc).AddTicks(4238) });

            migrationBuilder.UpdateData(
                schema: "auth",
                table: "roles",
                keyColumn: "id",
                keyValue: 2,
                columns: new[] { "updated_at", "created_at" },
                values: new object[] { new DateTime(2025, 8, 18, 17, 57, 4, 879, DateTimeKind.Utc).AddTicks(5100), new DateTime(2025, 8, 18, 17, 57, 4, 879, DateTimeKind.Utc).AddTicks(5100) });

            migrationBuilder.UpdateData(
                schema: "auth",
                table: "roles",
                keyColumn: "id",
                keyValue: 3,
                columns: new[] { "updated_at", "created_at" },
                values: new object[] { new DateTime(2025, 8, 18, 17, 57, 4, 879, DateTimeKind.Utc).AddTicks(5103), new DateTime(2025, 8, 18, 17, 57, 4, 879, DateTimeKind.Utc).AddTicks(5103) });

            migrationBuilder.UpdateData(
                schema: "auth",
                table: "roles",
                keyColumn: "id",
                keyValue: 4,
                columns: new[] { "updated_at", "created_at" },
                values: new object[] { new DateTime(2025, 8, 18, 17, 57, 4, 879, DateTimeKind.Utc).AddTicks(5104), new DateTime(2025, 8, 18, 17, 57, 4, 879, DateTimeKind.Utc).AddTicks(5104) });
        }
    }
}
