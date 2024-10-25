using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class FixedFavoritesAndImpulses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_Favorites_FavoritesModelUserId_FavoritesModelOpportunityId",
                table: "Opportunities");

            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_Impulses_ImpulseModelUserId_ImpulseModelOpportunityId",
                table: "Opportunities");

            migrationBuilder.DropTable(
                name: "FavoritesModelUserModel");

            migrationBuilder.DropTable(
                name: "ImpulseModelUserModel");

            migrationBuilder.DropIndex(
                name: "IX_Opportunities_FavoritesModelUserId_FavoritesModelOpportunityId",
                table: "Opportunities");

            migrationBuilder.DropIndex(
                name: "IX_Opportunities_ImpulseModelUserId_ImpulseModelOpportunityId",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "FavoritesModelOpportunityId",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "FavoritesModelUserId",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "ImpulseModelOpportunityId",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "ImpulseModelUserId",
                table: "Opportunities");

            migrationBuilder.CreateIndex(
                name: "IX_Impulses_OpportunityId",
                table: "Impulses",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_OpportunityId",
                table: "Favorites",
                column: "OpportunityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Opportunities_OpportunityId",
                table: "Favorites",
                column: "OpportunityId",
                principalTable: "Opportunities",
                principalColumn: "OpportunityId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Users_UserId",
                table: "Favorites",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Impulses_Opportunities_OpportunityId",
                table: "Impulses",
                column: "OpportunityId",
                principalTable: "Opportunities",
                principalColumn: "OpportunityId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Impulses_Users_UserId",
                table: "Impulses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Opportunities_OpportunityId",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Users_UserId",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Impulses_Opportunities_OpportunityId",
                table: "Impulses");

            migrationBuilder.DropForeignKey(
                name: "FK_Impulses_Users_UserId",
                table: "Impulses");

            migrationBuilder.DropIndex(
                name: "IX_Impulses_OpportunityId",
                table: "Impulses");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_OpportunityId",
                table: "Favorites");

            migrationBuilder.AddColumn<int>(
                name: "FavoritesModelOpportunityId",
                table: "Opportunities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FavoritesModelUserId",
                table: "Opportunities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImpulseModelOpportunityId",
                table: "Opportunities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImpulseModelUserId",
                table: "Opportunities",
                type: "int",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_FavoritesModelUserId_FavoritesModelOpportunityId",
                table: "Opportunities",
                columns: new[] { "FavoritesModelUserId", "FavoritesModelOpportunityId" });

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_ImpulseModelUserId_ImpulseModelOpportunityId",
                table: "Opportunities",
                columns: new[] { "ImpulseModelUserId", "ImpulseModelOpportunityId" });

            migrationBuilder.CreateIndex(
                name: "IX_FavoritesModelUserModel_FavoritesUserId_FavoritesOpportunityId",
                table: "FavoritesModelUserModel",
                columns: new[] { "FavoritesUserId", "FavoritesOpportunityId" });

            migrationBuilder.CreateIndex(
                name: "IX_ImpulseModelUserModel_ImpulsemodelsUserId_ImpulsemodelsOpportunityId",
                table: "ImpulseModelUserModel",
                columns: new[] { "ImpulsemodelsUserId", "ImpulsemodelsOpportunityId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_Favorites_FavoritesModelUserId_FavoritesModelOpportunityId",
                table: "Opportunities",
                columns: new[] { "FavoritesModelUserId", "FavoritesModelOpportunityId" },
                principalTable: "Favorites",
                principalColumns: new[] { "UserId", "OpportunityId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_Impulses_ImpulseModelUserId_ImpulseModelOpportunityId",
                table: "Opportunities",
                columns: new[] { "ImpulseModelUserId", "ImpulseModelOpportunityId" },
                principalTable: "Impulses",
                principalColumns: new[] { "UserId", "OpportunityId" });
        }
    }
}
