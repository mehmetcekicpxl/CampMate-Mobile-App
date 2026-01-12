using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampMate.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddWebsiteUrlColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                table: "CampSites",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                table: "CampSites");
        }
    }
}
