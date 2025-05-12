using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StackBook.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHistory_Orders_OrderId",
                table: "OrderHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderHistory",
                table: "OrderHistory");

            migrationBuilder.RenameTable(
                name: "OrderHistory",
                newName: "OrderHistories");

            migrationBuilder.RenameIndex(
                name: "IX_OrderHistory_OrderId",
                table: "OrderHistories",
                newName: "IX_OrderHistories_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderHistories",
                table: "OrderHistories",
                column: "OrderHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHistories_Orders_OrderId",
                table: "OrderHistories",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHistories_Orders_OrderId",
                table: "OrderHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderHistories",
                table: "OrderHistories");

            migrationBuilder.RenameTable(
                name: "OrderHistories",
                newName: "OrderHistory");

            migrationBuilder.RenameIndex(
                name: "IX_OrderHistories_OrderId",
                table: "OrderHistory",
                newName: "IX_OrderHistory_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderHistory",
                table: "OrderHistory",
                column: "OrderHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHistory_Orders_OrderId",
                table: "OrderHistory",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
