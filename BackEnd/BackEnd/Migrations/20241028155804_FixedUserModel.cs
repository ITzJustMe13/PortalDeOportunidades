using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class FixedUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_Users_UserModelUserId",
                table: "Opportunities");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Reviews_ReviewModelReservationId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ReviewModelReservationId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Opportunities_UserModelUserId",
                table: "Opportunities");

            migrationBuilder.DropIndex(
                name: "IX_Impulses_OpportunityId",
                table: "Impulses");

            migrationBuilder.DropColumn(
                name: "ReviewModelReservationId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "UserModelUserId",
                table: "Opportunities");

            migrationBuilder.AlterColumn<string>(
                name: "HashedPassword",
                table: "Users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<int>(
                name: "CellPhoneNum",
                table: "Users",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);

            migrationBuilder.AlterColumn<float>(
                name: "Rating",
                table: "Reviews",
                type: "real",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ReservationId",
                table: "Reviews",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "date",
                table: "Opportunities",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "userID",
                table: "Opportunities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpireDate",
                table: "Impulses",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Impulses",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_userID",
                table: "Opportunities",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_Impulses_OpportunityId",
                table: "Impulses",
                column: "OpportunityId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_Users_userID",
                table: "Opportunities",
                column: "userID",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Reservations_ReservationId",
                table: "Reviews",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "reservationID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Opportunities_Users_userID",
                table: "Opportunities");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Reservations_ReservationId",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Opportunities_userID",
                table: "Opportunities");

            migrationBuilder.DropIndex(
                name: "IX_Impulses_OpportunityId",
                table: "Impulses");

            migrationBuilder.DropColumn(
                name: "date",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "userID",
                table: "Opportunities");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "Impulses");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Impulses");

            migrationBuilder.AlterColumn<string>(
                name: "HashedPassword",
                table: "Users",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CellPhoneNum",
                table: "Users",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                table: "Reviews",
                type: "int",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<int>(
                name: "ReservationId",
                table: "Reviews",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "ReviewModelReservationId",
                table: "Reservations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserModelUserId",
                table: "Opportunities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ReviewModelReservationId",
                table: "Reservations",
                column: "ReviewModelReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Opportunities_UserModelUserId",
                table: "Opportunities",
                column: "UserModelUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Impulses_OpportunityId",
                table: "Impulses",
                column: "OpportunityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Opportunities_Users_UserModelUserId",
                table: "Opportunities",
                column: "UserModelUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Reviews_ReviewModelReservationId",
                table: "Reservations",
                column: "ReviewModelReservationId",
                principalTable: "Reviews",
                principalColumn: "ReservationId");
        }
    }
}
