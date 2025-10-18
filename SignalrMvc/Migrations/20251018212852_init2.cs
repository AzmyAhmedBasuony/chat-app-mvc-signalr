using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SignalrMvc.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 18, 23, 28, 52, 56, DateTimeKind.Local).AddTicks(8434), "$2a$11$TvOzRw2Gu7L86U8.gJn1s.LPV39DNnPzf/.5qt8lJ7IyBCHKIOukS" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 18, 23, 28, 52, 56, DateTimeKind.Local).AddTicks(8439), "$2a$11$TvOzRw2Gu7L86U8.gJn1s.LPV39DNnPzf/.5qt8lJ7IyBCHKIOukS" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 18, 23, 28, 52, 56, DateTimeKind.Local).AddTicks(8442), "$2a$11$TvOzRw2Gu7L86U8.gJn1s.LPV39DNnPzf/.5qt8lJ7IyBCHKIOukS" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 18, 23, 28, 52, 56, DateTimeKind.Local).AddTicks(8444), "$2a$11$TvOzRw2Gu7L86U8.gJn1s.LPV39DNnPzf/.5qt8lJ7IyBCHKIOukS" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 18, 23, 28, 52, 56, DateTimeKind.Local).AddTicks(8447), "$2a$11$TvOzRw2Gu7L86U8.gJn1s.LPV39DNnPzf/.5qt8lJ7IyBCHKIOukS" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 18, 23, 16, 20, 765, DateTimeKind.Local).AddTicks(4967), "$2a$11$5NH7miJjFp0mbvBrvrHxb.hIckRoKL.DDta8gwKgj9RI0UOc2wa5C" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 18, 23, 16, 20, 765, DateTimeKind.Local).AddTicks(4973), "$2a$11$5NH7miJjFp0mbvBrvrHxb.hIckRoKL.DDta8gwKgj9RI0UOc2wa5C" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 18, 23, 16, 20, 765, DateTimeKind.Local).AddTicks(4976), "$2a$11$5NH7miJjFp0mbvBrvrHxb.hIckRoKL.DDta8gwKgj9RI0UOc2wa5C" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 18, 23, 16, 20, 765, DateTimeKind.Local).AddTicks(4978), "$2a$11$5NH7miJjFp0mbvBrvrHxb.hIckRoKL.DDta8gwKgj9RI0UOc2wa5C" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2025, 10, 18, 23, 16, 20, 765, DateTimeKind.Local).AddTicks(4981), "$2a$11$5NH7miJjFp0mbvBrvrHxb.hIckRoKL.DDta8gwKgj9RI0UOc2wa5C" });
        }
    }
}
