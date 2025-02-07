using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "RegexValidations",
                newName: "RegexID");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "ContestFieldDetails",
                newName: "FieldDetailID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RegexID",
                table: "RegexValidations",
                newName: "ID");

            migrationBuilder.RenameColumn(
                name: "FieldDetailID",
                table: "ContestFieldDetails",
                newName: "ID");
        }
    }
}
