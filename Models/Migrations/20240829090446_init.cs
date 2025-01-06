using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "Contests",
                columns: table => new
                {
                    ContestID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContestUniqueCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NameContest = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescriptionContest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Keyword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TerminationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AppId = table.Column<int>(type: "int", nullable: false),
                    AppSecret = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidSmsresponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvalidSmsresponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepeatedSmsresponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidWhatsappResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvalidWhatsappResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepeatedWhatsappResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidOnlinePageResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepeatedOnlinePageResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidOnlineCompletionResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MissingFieldResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntryExclusionFields = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WinnerExclusionFields = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contests", x => x.ContestID);
                });

            migrationBuilder.CreateTable(
                name: "RegexValidations",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pattern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegexValidations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ContestFieldDetails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsOnlinePage = table.Column<bool>(type: "bit", nullable: true),
                    IsOnlineCompletion = table.Column<bool>(type: "bit", nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: true),
                    FieldLabel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldID = table.Column<int>(type: "int", nullable: false),
                    ContestID = table.Column<int>(type: "int", nullable: false),
                    RegexID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegexValidationID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestFieldDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ContestFieldDetails_ContestFields_FieldID",
                        column: x => x.FieldID,
                        principalTable: "ContestFields",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContestFieldDetails_Contests_ContestID",
                        column: x => x.ContestID,
                        principalTable: "Contests",
                        principalColumn: "ContestID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContestFieldDetails_RegexValidations_RegexValidationID",
                        column: x => x.RegexValidationID,
                        principalTable: "RegexValidations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContestFieldDetails_ContestID",
                table: "ContestFieldDetails",
                column: "ContestID");

            migrationBuilder.CreateIndex(
                name: "IX_ContestFieldDetails_FieldID",
                table: "ContestFieldDetails",
                column: "FieldID");

            migrationBuilder.CreateIndex(
                name: "IX_ContestFieldDetails_RegexValidationID",
                table: "ContestFieldDetails",
                column: "RegexValidationID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContestFieldDetails");

            migrationBuilder.DropTable(
                name: "ContestFields");

            migrationBuilder.DropTable(
                name: "Contests");

            migrationBuilder.DropTable(
                name: "RegexValidations");
        }
    }
}
