using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProspectbdApprovalDesk.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectContactName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "projects",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "projects");
        }
    }
}
