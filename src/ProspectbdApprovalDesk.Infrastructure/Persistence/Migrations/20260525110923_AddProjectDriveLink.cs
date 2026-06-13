using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProspectbdApprovalDesk.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectDriveLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DriveLink",
                table: "projects",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriveLink",
                table: "projects");
        }
    }
}
