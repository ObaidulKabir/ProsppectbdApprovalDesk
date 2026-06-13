using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProspectbdApprovalDesk.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProjectName",
                table: "projects",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProjectName",
                table: "projects");
        }
    }
}
