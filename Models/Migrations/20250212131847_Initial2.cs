using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "RegexValidations",
                columns: new[] { "RegexID", "Description", "Name", "Pattern" },
                values: new object[] { -3, "Regex for Receipt Number", "Receipt Number", "^\\S*\\d\\S*$" });

            migrationBuilder.InsertData(
                table: "RegexValidations",
                columns: new[] { "RegexID", "Description", "Name", "Pattern" },
                values: new object[] { -2, "Regex for Mobile Number", "Mobile Number", "^\\+*\\d+$" });

            migrationBuilder.InsertData(
                table: "RegexValidations",
                columns: new[] { "RegexID", "Description", "Name", "Pattern" },
                values: new object[] { -1, "Regex for Name", "Name", "^[a-zA-Z ]+$" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegexValidations",
                keyColumn: "RegexID",
                keyValue: -3);

            migrationBuilder.DeleteData(
                table: "RegexValidations",
                keyColumn: "RegexID",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "RegexValidations",
                keyColumn: "RegexID",
                keyValue: -1);
        }
    }
}
