using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travellark.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToDestination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Destinations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Destinations");
        }
    }
}
