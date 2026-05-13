using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RequirementTrackingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "application_user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "char(36)", maxLength: 36, nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_application_user", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "robot",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    webhook_url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    group_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_enabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_robot", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    username = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    real_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    role = table.Column<int>(type: "int", nullable: false),
                    phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_enabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "project",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    manager_id = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project", x => x.id);
                    table.ForeignKey(
                        name: "FK_project_user_manager_id",
                        column: x => x.manager_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "requirement",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    requirement_no = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<int>(type: "int", nullable: false),
                    progress = table.Column<int>(type: "int", nullable: false),
                    follower_id = table.Column<int>(type: "int", nullable: false),
                    plan_start_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    plan_test_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    plan_launch_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    actual_test_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    actual_launch_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    is_confirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    doc_url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    price = table.Column<decimal>(type: "decimal(65,30)", precision: 18, scale: 2, nullable: true),
                    project_id = table.Column<int>(type: "int", nullable: false),
                    robot_id = table.Column<int>(type: "int", nullable: true),
                    priority = table.Column<int>(type: "int", nullable: false),
                    remark = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    version = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requirement", x => x.id);
                    table.ForeignKey(
                        name: "FK_requirement_project_project_id",
                        column: x => x.project_id,
                        principalTable: "project",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_requirement_robot_robot_id",
                        column: x => x.robot_id,
                        principalTable: "robot",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_requirement_user_follower_id",
                        column: x => x.follower_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "notification_job",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    requirement_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    scheduled_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    sent_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_job", x => x.id);
                    table.ForeignKey(
                        name: "FK_notification_job_requirement_requirement_id",
                        column: x => x.requirement_id,
                        principalTable: "requirement",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "notification_log",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    requirement_id = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    robot_id = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false),
                    sent_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    error_message = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    retry_count = table.Column<int>(type: "int", nullable: false),
                    last_attempt_at = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification_log", x => x.id);
                    table.ForeignKey(
                        name: "FK_notification_log_requirement_requirement_id",
                        column: x => x.requirement_id,
                        principalTable: "requirement",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_notification_log_robot_robot_id",
                        column: x => x.robot_id,
                        principalTable: "robot",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_notification_job_requirement_id",
                table: "notification_job",
                column: "requirement_id");

            migrationBuilder.CreateIndex(
                name: "IX_notification_log_requirement_id",
                table: "notification_log",
                column: "requirement_id");

            migrationBuilder.CreateIndex(
                name: "IX_notification_log_robot_id",
                table: "notification_log",
                column: "robot_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_code",
                table: "project",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_project_manager_id",
                table: "project",
                column: "manager_id");

            migrationBuilder.CreateIndex(
                name: "IX_requirement_follower_id",
                table: "requirement",
                column: "follower_id");

            migrationBuilder.CreateIndex(
                name: "IX_requirement_project_id",
                table: "requirement",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "IX_requirement_requirement_no",
                table: "requirement",
                column: "requirement_no",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_requirement_robot_id",
                table: "requirement",
                column: "robot_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_username",
                table: "user",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "application_user");

            migrationBuilder.DropTable(
                name: "notification_job");

            migrationBuilder.DropTable(
                name: "notification_log");

            migrationBuilder.DropTable(
                name: "requirement");

            migrationBuilder.DropTable(
                name: "project");

            migrationBuilder.DropTable(
                name: "robot");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
