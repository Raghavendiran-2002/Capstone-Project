using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizApi.Migrations
{
    public partial class inits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Background",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Music",
                table: "Quizzes");

            migrationBuilder.AddColumn<bool>(
                name: "DurationPerQuestion",
                table: "Quizzes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationPerQuestion",
                table: "Quizzes");

            migrationBuilder.AddColumn<string>(
                name: "Background",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Music",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
