using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PocketBookServer.Migrations
{
    /// <inheritdoc />
    public partial class initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PocketTransaction",
                table: "PocketTransaction");

            migrationBuilder.RenameTable(
                name: "PocketTransaction",
                newName: "PocketTransactions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PocketTransactions",
                table: "PocketTransactions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    IsIncome = table.Column<bool>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    DetailedInfo = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PocketTransactions",
                table: "PocketTransactions");

            migrationBuilder.RenameTable(
                name: "PocketTransactions",
                newName: "PocketTransaction");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PocketTransaction",
                table: "PocketTransaction",
                column: "Id");
        }
    }
}
