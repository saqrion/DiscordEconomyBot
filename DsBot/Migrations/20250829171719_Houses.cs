using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DsBot.Migrations
{
    /// <inheritdoc />
    public partial class Houses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "HouseId",
                table: "Users",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Houses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Score = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<ulong>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Houses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_HouseId",
                table: "Users",
                column: "HouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Houses_HouseId",
                table: "Users",
                column: "HouseId",
                principalTable: "Houses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Houses_HouseId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Houses");

            migrationBuilder.DropIndex(
                name: "IX_Users_HouseId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "HouseId",
                table: "Users");
        }
    }
}
