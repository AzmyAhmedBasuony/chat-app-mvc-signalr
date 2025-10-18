using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SignalrMvc.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAuthAndPrivateMessaging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProfileImage = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsOnline = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    LastSeen = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrivateMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    ReceiverId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivateMessages_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PrivateMessages_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserConnections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ConnectionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConnectedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserConnections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserConnections_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FullName", "LastSeen", "PasswordHash", "ProfileImage", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 18, 23, 16, 20, 765, DateTimeKind.Local).AddTicks(4967), "alice@chat.com", "Alice Johnson", null, "$2a$11$5NH7miJjFp0mbvBrvrHxb.hIckRoKL.DDta8gwKgj9RI0UOc2wa5C", "https://ui-avatars.com/api/?name=Alice+Johnson&background=667eea&color=fff", "alice" },
                    { 2, new DateTime(2025, 10, 18, 23, 16, 20, 765, DateTimeKind.Local).AddTicks(4973), "bob@chat.com", "Bob Smith", null, "$2a$11$5NH7miJjFp0mbvBrvrHxb.hIckRoKL.DDta8gwKgj9RI0UOc2wa5C", "https://ui-avatars.com/api/?name=Bob+Smith&background=764ba2&color=fff", "bob" },
                    { 3, new DateTime(2025, 10, 18, 23, 16, 20, 765, DateTimeKind.Local).AddTicks(4976), "charlie@chat.com", "Charlie Brown", null, "$2a$11$5NH7miJjFp0mbvBrvrHxb.hIckRoKL.DDta8gwKgj9RI0UOc2wa5C", "https://ui-avatars.com/api/?name=Charlie+Brown&background=f093fb&color=fff", "charlie" },
                    { 4, new DateTime(2025, 10, 18, 23, 16, 20, 765, DateTimeKind.Local).AddTicks(4978), "diana@chat.com", "Diana Prince", null, "$2a$11$5NH7miJjFp0mbvBrvrHxb.hIckRoKL.DDta8gwKgj9RI0UOc2wa5C", "https://ui-avatars.com/api/?name=Diana+Prince&background=4facfe&color=fff", "diana" },
                    { 5, new DateTime(2025, 10, 18, 23, 16, 20, 765, DateTimeKind.Local).AddTicks(4981), "eve@chat.com", "Eve Davis", null, "$2a$11$5NH7miJjFp0mbvBrvrHxb.hIckRoKL.DDta8gwKgj9RI0UOc2wa5C", "https://ui-avatars.com/api/?name=Eve+Davis&background=43e97b&color=fff", "eve" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrivateMessages_ReceiverId_IsRead",
                table: "PrivateMessages",
                columns: new[] { "ReceiverId", "IsRead" });

            migrationBuilder.CreateIndex(
                name: "IX_PrivateMessages_SenderId_ReceiverId_SentAt",
                table: "PrivateMessages",
                columns: new[] { "SenderId", "ReceiverId", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PrivateMessages_SentAt",
                table: "PrivateMessages",
                column: "SentAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserConnections_ConnectionId",
                table: "UserConnections",
                column: "ConnectionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserConnections_UserId",
                table: "UserConnections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsOnline",
                table: "Users",
                column: "IsOnline");

            migrationBuilder.CreateIndex(
                name: "IX_Users_LastSeen",
                table: "Users",
                column: "LastSeen");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrivateMessages");

            migrationBuilder.DropTable(
                name: "UserConnections");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
