using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class initial3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "RegexValidations",
                keyColumn: "RegexID",
                keyValue: -3,
                columns: new[] { "Description", "Name", "Pattern" },
                values: new object[] { "Regex for Mobile Number", "Mobile Number", "^\\+*\\d+$" });

            migrationBuilder.UpdateData(
                table: "RegexValidations",
                keyColumn: "RegexID",
                keyValue: -2,
                columns: new[] { "Description", "Name", "Pattern" },
                values: new object[] { "Regex for Name", "Name", "^[a-zA-Z ]+$" });

            migrationBuilder.UpdateData(
                table: "RegexValidations",
                keyColumn: "RegexID",
                keyValue: -1,
                columns: new[] { "Description", "Name", "Pattern" },
                values: new object[] { "No Regex", "No Regex", "" });

            migrationBuilder.InsertData(
                table: "RegexValidations",
                columns: new[] { "RegexID", "Description", "Name", "Pattern" },
                values: new object[] { -4, "Regex for Receipt Number", "Receipt Number", "^\\S*\\d\\S*$" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RegexValidations",
                keyColumn: "RegexID",
                keyValue: -4);

            migrationBuilder.UpdateData(
                table: "RegexValidations",
                keyColumn: "RegexID",
                keyValue: -3,
                columns: new[] { "Description", "Name", "Pattern" },
                values: new object[] { "Regex for Receipt Number", "Receipt Number", "^\\S*\\d\\S*$" });

            migrationBuilder.UpdateData(
                table: "RegexValidations",
                keyColumn: "RegexID",
                keyValue: -2,
                columns: new[] { "Description", "Name", "Pattern" },
                values: new object[] { "Regex for Mobile Number", "Mobile Number", "^\\+*\\d+$" });

            migrationBuilder.UpdateData(
                table: "RegexValidations",
                keyColumn: "RegexID",
                keyValue: -1,
                columns: new[] { "Description", "Name", "Pattern" },
                values: new object[] { "Regex for Name", "Name", "^[a-zA-Z ]+$" });
        }
    }
}
