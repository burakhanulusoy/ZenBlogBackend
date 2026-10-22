using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZenBlog.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_account_status_and_user_blocks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HiddenByAccount",
                table: "SubComments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HiddenByAccount",
                table: "Comments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HiddenByAccount",
                table: "Blogs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AccountStatus",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionReminderSentAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionRequestedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockedByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    EvidenceUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BlockedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnblockedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UnblockedByAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UnblockReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBlocks_AspNetUsers_BlockedByAdminId",
                        column: x => x.BlockedByAdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserBlocks_AspNetUsers_UnblockedByAdminId",
                        column: x => x.UnblockedByAdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserBlocks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_BlockedByAdminId",
                table: "UserBlocks",
                column: "BlockedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_UnblockedByAdminId",
                table: "UserBlocks",
                column: "UnblockedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBlocks_UserId",
                table: "UserBlocks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBlocks");

            migrationBuilder.DropColumn(
                name: "HiddenByAccount",
                table: "SubComments");

            migrationBuilder.DropColumn(
                name: "HiddenByAccount",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "HiddenByAccount",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "AccountStatus",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DeletionReminderSentAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DeletionRequestedAt",
                table: "AspNetUsers");
        }
    }
}
