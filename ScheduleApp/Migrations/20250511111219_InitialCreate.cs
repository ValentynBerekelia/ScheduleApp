using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Disable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.RoomId);
                });

            migrationBuilder.CreateTable(
                name: "Roots",
                columns: table => new
                {
                    RootId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roots", x => x.RootId);
                });

            migrationBuilder.CreateTable(
                name: "SaturdayClasses",
                columns: table => new
                {
                    SaturdayId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartSaturday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndSaturday = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WeekType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SecondWeekType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaturdayClasses", x => x.SaturdayId);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    TeacherId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Disable = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Patronymic = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.TeacherId);
                    table.ForeignKey(
                        name: "FK_Teachers_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId");
                });

            migrationBuilder.CreateTable(
                name: "Schedules",
                columns: table => new
                {
                    ScheduleItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RootId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schedules", x => x.ScheduleItemId);
                    table.ForeignKey(
                        name: "FK_Schedules_Roots_RootId",
                        column: x => x.RootId,
                        principalTable: "Roots",
                        principalColumn: "RootId");
                });

            migrationBuilder.CreateTable(
                name: "Semesters",
                columns: table => new
                {
                    SemesterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Year = table.Column<int>(type: "int", nullable: true),
                    StartDay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndDay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentSemester = table.Column<bool>(type: "bit", nullable: false),
                    DefaultSemester = table.Column<bool>(type: "bit", nullable: false),
                    Disable = table.Column<bool>(type: "bit", nullable: false),
                    SemesterDays = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RootId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Semesters", x => x.SemesterId);
                    table.ForeignKey(
                        name: "FK_Semesters_Roots_RootId",
                        column: x => x.RootId,
                        principalTable: "Roots",
                        principalColumn: "RootId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    LessonId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeacherId = table.Column<int>(type: "int", nullable: true),
                    LinkToMeeting = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubjectForSite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LessonType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoomId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.LessonId);
                    table.ForeignKey(
                        name: "FK_Lessons_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "RoomId");
                    table.ForeignKey(
                        name: "FK_Lessons_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "TeacherId");
                });

            migrationBuilder.CreateTable(
                name: "Days",
                columns: table => new
                {
                    DayId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    day = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScheduleItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Days", x => x.DayId);
                    table.ForeignKey(
                        name: "FK_Days_Schedules_ScheduleItemId",
                        column: x => x.ScheduleItemId,
                        principalTable: "Schedules",
                        principalColumn: "ScheduleItemId");
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Disable = table.Column<bool>(type: "bit", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ScheduleItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.GroupId);
                    table.ForeignKey(
                        name: "FK_Groups_Schedules_ScheduleItemId",
                        column: x => x.ScheduleItemId,
                        principalTable: "Schedules",
                        principalColumn: "ScheduleItemId");
                });

            migrationBuilder.CreateTable(
                name: "SemesterClasses",
                columns: table => new
                {
                    SemesterClassId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EndTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClassName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SemesterId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemesterClasses", x => x.SemesterClassId);
                    table.ForeignKey(
                        name: "FK_SemesterClasses_Semesters_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semesters",
                        principalColumn: "SemesterId");
                });

            migrationBuilder.CreateTable(
                name: "Weeks",
                columns: table => new
                {
                    WeeksId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EvenLessonId = table.Column<int>(type: "int", nullable: true),
                    OddLessonId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weeks", x => x.WeeksId);
                    table.ForeignKey(
                        name: "FK_Weeks_Lessons_EvenLessonId",
                        column: x => x.EvenLessonId,
                        principalTable: "Lessons",
                        principalColumn: "LessonId");
                    table.ForeignKey(
                        name: "FK_Weeks_Lessons_OddLessonId",
                        column: x => x.OddLessonId,
                        principalTable: "Lessons",
                        principalColumn: "LessonId");
                });

            migrationBuilder.CreateTable(
                name: "ClassItems",
                columns: table => new
                {
                    ClassItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WeeksId = table.Column<int>(type: "int", nullable: true),
                    ClassSemesterClassId = table.Column<int>(type: "int", nullable: true),
                    DayId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassItems", x => x.ClassItemID);
                    table.ForeignKey(
                        name: "FK_ClassItems_Days_DayId",
                        column: x => x.DayId,
                        principalTable: "Days",
                        principalColumn: "DayId");
                    table.ForeignKey(
                        name: "FK_ClassItems_SemesterClasses_ClassSemesterClassId",
                        column: x => x.ClassSemesterClassId,
                        principalTable: "SemesterClasses",
                        principalColumn: "SemesterClassId");
                    table.ForeignKey(
                        name: "FK_ClassItems_Weeks_WeeksId",
                        column: x => x.WeeksId,
                        principalTable: "Weeks",
                        principalColumn: "WeeksId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassItems_ClassSemesterClassId",
                table: "ClassItems",
                column: "ClassSemesterClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassItems_DayId",
                table: "ClassItems",
                column: "DayId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassItems_WeeksId",
                table: "ClassItems",
                column: "WeeksId");

            migrationBuilder.CreateIndex(
                name: "IX_Days_ScheduleItemId",
                table: "Days",
                column: "ScheduleItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ScheduleItemId",
                table: "Groups",
                column: "ScheduleItemId",
                unique: true,
                filter: "[ScheduleItemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_RoomId",
                table: "Lessons",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_TeacherId",
                table: "Lessons",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_RootId",
                table: "Schedules",
                column: "RootId");

            migrationBuilder.CreateIndex(
                name: "IX_SemesterClasses_SemesterId",
                table: "SemesterClasses",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Semesters_RootId",
                table: "Semesters",
                column: "RootId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_DepartmentId",
                table: "Teachers",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_EvenLessonId",
                table: "Weeks",
                column: "EvenLessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Weeks_OddLessonId",
                table: "Weeks",
                column: "OddLessonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassItems");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "SaturdayClasses");

            migrationBuilder.DropTable(
                name: "Days");

            migrationBuilder.DropTable(
                name: "SemesterClasses");

            migrationBuilder.DropTable(
                name: "Weeks");

            migrationBuilder.DropTable(
                name: "Schedules");

            migrationBuilder.DropTable(
                name: "Semesters");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "Roots");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
