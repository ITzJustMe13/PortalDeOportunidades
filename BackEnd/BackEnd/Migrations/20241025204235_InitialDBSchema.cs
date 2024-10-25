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
                name: "Favorites",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OpportunityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => new { x.UserId, x.OpportunityId });
                });

            migrationBuilder.CreateTable(
                name: "Impulses",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OpportunityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Impulses", x => new { x.UserId, x.OpportunityId });
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReservationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Desc = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReservationId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HashedPassword = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ExternalId = table.Column<int>(type: "int", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CellPhoneNum = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
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
                name: "FavoritesModelUserModel",
                columns: table => new
                {
                    UsersUserId = table.Column<int>(type: "int", nullable: false),
                    FavoritesUserId = table.Column<int>(type: "int", nullable: false),
                    FavoritesOpportunityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoritesModelUserModel", x => new { x.UsersUserId, x.FavoritesUserId, x.FavoritesOpportunityId });
                    table.ForeignKey(
                        name: "FK_FavoritesModelUserModel_Favorites_FavoritesUserId_FavoritesOpportunityId",
                        columns: x => new { x.FavoritesUserId, x.FavoritesOpportunityId },
                        principalTable: "Favorites",
                        principalColumns: new[] { "UserId", "OpportunityId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavoritesModelUserModel_Users_UsersUserId",
                        column: x => x.UsersUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImpulseModelUserModel",
                columns: table => new
                {
                    UsersUserId = table.Column<int>(type: "int", nullable: false),
                    ImpulsemodelsUserId = table.Column<int>(type: "int", nullable: false),
                    ImpulsemodelsOpportunityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImpulseModelUserModel", x => new { x.UsersUserId, x.ImpulsemodelsUserId, x.ImpulsemodelsOpportunityId });
                    table.ForeignKey(
                        name: "FK_ImpulseModelUserModel_Impulses_ImpulsemodelsUserId_ImpulsemodelsOpportunityId",
                        columns: x => new { x.ImpulsemodelsUserId, x.ImpulsemodelsOpportunityId },
                        principalTable: "Impulses",
                        principalColumns: new[] { "UserId", "OpportunityId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImpulseModelUserModel_Users_UsersUserId",
                        column: x => x.UsersUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Opportunities",
                columns: table => new
                {
                    OpportunityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    Vacancies = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Score = table.Column<float>(type: "real", nullable: false),
                    IsImpulsed = table.Column<bool>(type: "bit", nullable: false),
                    FavoritesModelOpportunityId = table.Column<int>(type: "int", nullable: true),
                    FavoritesModelUserId = table.Column<int>(type: "int", nullable: true),
                    ImpulseModelOpportunityId = table.Column<int>(type: "int", nullable: true),
                    ImpulseModelUserId = table.Column<int>(type: "int", nullable: true),
                    UserModelUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opportunities", x => x.OpportunityId);
                    table.ForeignKey(
                        name: "FK_Opportunities_Favorites_FavoritesModelUserId_FavoritesModelOpportunityId",
                        columns: x => new { x.FavoritesModelUserId, x.FavoritesModelOpportunityId },
                        principalTable: "Favorites",
                        principalColumns: new[] { "UserId", "OpportunityId" });
                    table.ForeignKey(
                        name: "FK_Opportunities_Impulses_ImpulseModelUserId_ImpulseModelOpportunityId",
                        columns: x => new { x.ImpulseModelUserId, x.ImpulseModelOpportunityId },
                        principalTable: "Impulses",
                        principalColumns: new[] { "UserId", "OpportunityId" });
                    table.ForeignKey(
                        name: "FK_Opportunities_Users_UserModelUserId",
                        column: x => x.UserModelUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
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
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    ReviewModelReservationId = table.Column<int>(type: "int", nullable: true)
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
                        name: "FK_Reservations_Reviews_ReviewModelReservationId",
                        column: x => x.ReviewModelReservationId,
                        principalTable: "Reviews",
                        principalColumn: "ReservationId");
                    table.ForeignKey(
                        name: "FK_Reservations_Users_userID",
                        column: x => x.userID,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavoritesModelUserModel_FavoritesUserId_FavoritesOpportunityId",
                table: "FavoritesModelUserModel",
                columns: new[] { "FavoritesUserId", "FavoritesOpportunityId" });

            migrationBuilder.CreateIndex(
                name: "IX_ImpulseModelUserModel_ImpulsemodelsUserId_ImpulsemodelsOpportunityId",
                table: "ImpulseModelUserModel",
                columns: new[] { "ImpulsemodelsUserId", "ImpulsemodelsOpportunityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_FavoritesModelUserId_FavoritesModelOpportunityId",
                table: "Opportunities",
                columns: new[] { "FavoritesModelUserId", "FavoritesModelOpportunityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_ImpulseModelUserId_ImpulseModelOpportunityId",
                table: "Opportunities",
                columns: new[] { "ImpulseModelUserId", "ImpulseModelOpportunityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_UserModelUserId",
                table: "Opportunities",
                column: "UserModelUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_opportunityID",
                table: "Reservations",
                column: "opportunityID");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ReviewModelReservationId",
                table: "Reservations",
                column: "ReviewModelReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_userID",
                table: "Reservations",
                column: "userID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavoritesModelUserModel");

            migrationBuilder.DropTable(
                name: "ImpulseModelUserModel");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Opportunities");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "Impulses");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
