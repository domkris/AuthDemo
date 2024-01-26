using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthDemo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFinishedAndApprovedToChore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Chores",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFinished",
                table: "Chores",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Chores");

            migrationBuilder.DropColumn(
                name: "IsFinished",
                table: "Chores");
        }
    }
}
