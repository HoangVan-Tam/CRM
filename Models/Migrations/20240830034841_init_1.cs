using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class init_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsOnlinePage",
                table: "ContestFieldDetails",
                newName: "ShowOnlinePage");

            migrationBuilder.RenameColumn(
                name: "IsOnlineCompletion",
                table: "ContestFieldDetails",
                newName: "ShowOnlineCompletion");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "ContestFieldDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "ContestFieldDetails");

            migrationBuilder.RenameColumn(
                name: "ShowOnlinePage",
                table: "ContestFieldDetails",
                newName: "IsOnlinePage");

            migrationBuilder.RenameColumn(
                name: "ShowOnlineCompletion",
                table: "ContestFieldDetails",
                newName: "IsOnlineCompletion");
        }
    }
}
