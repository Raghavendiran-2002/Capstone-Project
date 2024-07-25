using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizApi.Migrations
{
    public partial class Inits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "Tags",
                column: "TagName",
                values: new object[]
                {
                    "App Development",
                    "Devops",
                    "Network Engineer",
                    "System Admin"
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "TagName",
                keyValue: "App Development");

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "TagName",
                keyValue: "Devops");

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "TagName",
                keyValue: "Network Engineer");

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "TagName",
                keyValue: "System Admin");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
