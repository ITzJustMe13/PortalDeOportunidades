using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class InitialDBSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HashedPassword = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExternalId = table.Column<int>(type: "int", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CellPhoneNum = table.Column<int>(type: "int", maxLength: 9, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TokenExpDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Opportunities",
                columns: table => new
                {
                    OpportunityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    Vacancies = table.Column<int>(type: "int", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Score = table.Column<float>(type: "real", nullable: false),
                    IsImpulsed = table.Column<bool>(type: "bit", nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opportunities", x => x.OpportunityId);
                    table.ForeignKey(
                        name: "FK_Opportunities_Users_userID",
                        column: x => x.userID,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OpportunityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => new { x.UserId, x.OpportunityId });
                    table.ForeignKey(
                        name: "FK_Favorites_Opportunities_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunities",
                        principalColumn: "OpportunityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Impulses",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OpportunityId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Impulses", x => new { x.UserId, x.OpportunityId });
                    table.ForeignKey(
                        name: "FK_Impulses_Opportunities_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "Opportunities",
                        principalColumn: "OpportunityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Impulses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    reservationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    opportunityID = table.Column<int>(type: "int", nullable: false),
                    userID = table.Column<int>(type: "int", nullable: false),
                    reservationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    checkInDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    numOfPeople = table.Column<int>(type: "int", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.reservationID);
                    table.ForeignKey(
                        name: "FK_Reservations_Opportunities_opportunityID",
                        column: x => x.opportunityID,
                        principalTable: "Opportunities",
                        principalColumn: "OpportunityId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Users_userID",
                        column: x => x.userID,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<float>(type: "real", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReservationId);
                    table.ForeignKey(
                        name: "FK_Reviews_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "reservationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_OpportunityId",
                table: "Favorites",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Impulses_OpportunityId",
                table: "Impulses",
                column: "OpportunityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_userID",
                table: "Opportunities",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_opportunityID",
                table: "Reservations",
                column: "opportunityID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_userID",
                table: "Reservations",
                column: "userID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "Impulses");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Opportunities");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
