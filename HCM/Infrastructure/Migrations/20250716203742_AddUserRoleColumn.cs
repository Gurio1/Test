using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HCM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRoleColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "ApplicationUsers",
                newName: "Username");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "ApplicationUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "ApplicationUsers");

            migrationBuilder.RenameColumn(
                name: "Username",
                table: "ApplicationUsers",
                newName: "Email");
        }
    }
}
