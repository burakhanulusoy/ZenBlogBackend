using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZenBlog.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_blog_views_and_user_created_at : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Blogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.CreateTable(
                name: "BlogViews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VisitorKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ViewDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogViews_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogViews_BlogId_VisitorKey_ViewDate",
                table: "BlogViews",
                columns: new[] { "BlogId", "VisitorKey", "ViewDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogViews_ViewDate",
                table: "BlogViews",
                column: "ViewDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogViews");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");
        }
    }
}
