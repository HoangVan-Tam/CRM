using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class Initial3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contests",
                columns: table => new
                {
                    ContestID = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    SMSSubmitFields = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValidationRegexFull = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidSmsresponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvalidSmsresponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepeatedSmsresponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidWhatsappResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvalidWhatsappResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepeatedWhatsappResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidOnlinePageResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepeatedOnlinePageResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ValidOnlineCompletionResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ErrorMessageAmount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MissingFieldResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntryExclusionFields = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WinnerExclusionFields = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TierAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
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
                    FieldDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShowOnlinePage = table.Column<bool>(type: "bit", nullable: true),
                    ShowOnlineCompletion = table.Column<bool>(type: "bit", nullable: true),
                    IsRequired = table.Column<bool>(type: "bit", nullable: true),
                    FieldLabel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormControl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ContestID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RegexValidationID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContestFieldDetails", x => x.FieldDetailID);
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
                name: "IX_ContestFieldDetails_RegexValidationID",
                table: "ContestFieldDetails",
                column: "RegexValidationID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContestFieldDetails");

            migrationBuilder.DropTable(
                name: "Contests");

            migrationBuilder.DropTable(
                name: "RegexValidations");
        }
    }
}
