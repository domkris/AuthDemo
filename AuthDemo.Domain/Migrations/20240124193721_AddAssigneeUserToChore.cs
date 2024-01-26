using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthDemo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAssigneeUserToChore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "UserAssigneeId",
                table: "Chores",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chores_UserAssigneeId",
                table: "Chores",
                column: "UserAssigneeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chores_AspNetUsers_UserAssigneeId",
                table: "Chores",
                column: "UserAssigneeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chores_AspNetUsers_UserAssigneeId",
                table: "Chores");

            migrationBuilder.DropIndex(
                name: "IX_Chores_UserAssigneeId",
                table: "Chores");

            migrationBuilder.DropColumn(
                name: "UserAssigneeId",
                table: "Chores");
        }
    }
}
