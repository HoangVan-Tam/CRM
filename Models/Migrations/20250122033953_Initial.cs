using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContestFieldDetails_ContestFields_FieldID",
                table: "ContestFieldDetails");

            migrationBuilder.DropTable(
                name: "ContestFields");

            migrationBuilder.DropIndex(
                name: "IX_ContestFieldDetails_FieldID",
                table: "ContestFieldDetails");

            migrationBuilder.DropColumn(
                name: "FieldID",
                table: "ContestFieldDetails");

            migrationBuilder.AddColumn<string>(
                name: "SMSSubmitFields",
                table: "Contests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ValidationRegexFull",
                table: "Contests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RegexID",
                table: "ContestFieldDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "FieldName",
                table: "ContestFieldDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FieldType",
                table: "ContestFieldDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormControl",
                table: "ContestFieldDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SMSSubmitFields",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "ValidationRegexFull",
                table: "Contests");

            migrationBuilder.DropColumn(
                name: "FieldName",
                table: "ContestFieldDetails");

            migrationBuilder.DropColumn(
                name: "FieldType",
                table: "ContestFieldDetails");

            migrationBuilder.DropColumn(
                name: "FormControl",
                table: "ContestFieldDetails");

            migrationBuilder.AlterColumn<string>(
                name: "RegexID",
                table: "ContestFieldDetails",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "FieldID",
                table: "ContestFieldDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ContestFields",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FieldName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FieldType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestFields", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContestFieldDetails_FieldID",
                table: "ContestFieldDetails",
                column: "FieldID");

            migrationBuilder.AddForeignKey(
                name: "FK_ContestFieldDetails_ContestFields_FieldID",
                table: "ContestFieldDetails",
                column: "FieldID",
                principalTable: "ContestFields",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
