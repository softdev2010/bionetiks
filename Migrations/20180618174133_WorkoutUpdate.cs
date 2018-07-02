using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FitnessApp.Migrations
{
    public partial class WorkoutUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_AspNetUsers_UserId",
                table: "Training");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Training",
                table: "Training");

            migrationBuilder.RenameTable(
                name: "Training",
                newName: "Trainings");

            migrationBuilder.RenameIndex(
                name: "IX_Training_UserId",
                table: "Trainings",
                newName: "IX_Trainings_UserId");

            migrationBuilder.AddColumn<string>(
                name: "MuscleGroup",
                table: "Trainings",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Trainings",
                table: "Trainings",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Workouts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TemplateId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    NumberOfRepetitions = table.Column<int>(nullable: false),
                    Duration = table.Column<double>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    IsSuccessfull = table.Column<bool>(nullable: false),
                    AverageRepetitionDuration = table.Column<double>(nullable: false),
                    AverageRepetitionAcceleration = table.Column<double>(nullable: false),
                    AverageTilt = table.Column<double>(nullable: false),
                    AccelerationValues = table.Column<string>(nullable: true),
                    VelocityValues = table.Column<string>(nullable: true),
                    TiltValues = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workouts_Trainings_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Trainings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Workouts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_TemplateId",
                table: "Workouts",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Workouts_UserId",
                table: "Workouts",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trainings_AspNetUsers_UserId",
                table: "Trainings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trainings_AspNetUsers_UserId",
                table: "Trainings");

            migrationBuilder.DropTable(
                name: "Workouts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Trainings",
                table: "Trainings");

            migrationBuilder.DropColumn(
                name: "MuscleGroup",
                table: "Trainings");

            migrationBuilder.RenameTable(
                name: "Trainings",
                newName: "Training");

            migrationBuilder.RenameIndex(
                name: "IX_Trainings_UserId",
                table: "Training",
                newName: "IX_Training_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Training",
                table: "Training",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_AspNetUsers_UserId",
                table: "Training",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
