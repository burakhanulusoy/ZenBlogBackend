using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZenBlog.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class mig_team_title_and_description_fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Descriptiron",
                table: "Teams",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Teams",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Teams");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Teams",
                newName: "Descriptiron");
        }
    }
}
