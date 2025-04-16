using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StackBook.Migrations
{
    /// <inheritdoc />
    public partial class FixBugFinally : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Books_BookId",
                table: "Carts");

            migrationBuilder.DropTable(
                name: "ReturnOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Carts_BookId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "ReturnOrders");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "CreatedCart",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Carts");

            migrationBuilder.CreateTable(
                name: "CartBooks",
                columns: table => new
                {
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedCart = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartBooks", x => new { x.CartId, x.BookId });
                    table.ForeignKey(
                        name: "FK_CartBooks_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartBooks_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "CartId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_CartBooks_BookId",
                table: "CartBooks",
                column: "BookId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartBooks");

            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderId",
                table: "Payments");

            migrationBuilder.AddColumn<double>(
                name: "TotalPrice",
                table: "ReturnOrders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<Guid>(
                name: "BookId",
                table: "Carts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedCart",
                table: "Carts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReturnOrderDetails",
                columns: table => new
                {
                    ReturnOrderDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReturnOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnOrderDetails", x => x.ReturnOrderDetailId);
                    table.ForeignKey(
                        name: "FK_ReturnOrderDetails_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReturnOrderDetails_ReturnOrders_ReturnOrderId",
                        column: x => x.ReturnOrderId,
                        principalTable: "ReturnOrders",
                        principalColumn: "ReturnOrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_BookId",
                table: "Carts",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnOrderDetails_BookId",
                table: "ReturnOrderDetails",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnOrderDetails_ReturnOrderId",
                table: "ReturnOrderDetails",
                column: "ReturnOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Books_BookId",
                table: "Carts",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
