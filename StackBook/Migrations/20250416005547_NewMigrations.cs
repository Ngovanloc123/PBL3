using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StackBook.Migrations
{
    /// <inheritdoc />
    public partial class NewMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_OrderDetails_OrderDetailId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ShippingAddresses_ShippingAddressId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Orders_PaymentId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnOrders_Orders_OrderId",
                table: "ReturnOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnOrders_ReturnOrderDetails_ReturnOrderDetailId",
                table: "ReturnOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingAddresses_Users_UserId",
                table: "ShippingAddresses");

            migrationBuilder.DropTable(
                name: "CartDetails");

            migrationBuilder.DropIndex(
                name: "IX_ReturnOrders_ReturnOrderDetailId",
                table: "ReturnOrders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderDetailId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ReturnOrderDetailId",
                table: "ReturnOrders");

            migrationBuilder.DropColumn(
                name: "Transaction",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OrderDetailId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ward",
                table: "ShippingAddresses",
                newName: "Ward");

            migrationBuilder.RenameColumn(
                name: "country",
                table: "ShippingAddresses",
                newName: "Country");

            migrationBuilder.RenameColumn(
                name: "city",
                table: "ShippingAddresses",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "reason",
                table: "ReturnOrders",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "quantity",
                table: "OrderDetails",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "Discounts",
                newName: "Price");

            migrationBuilder.AlterColumn<string>(
                name: "VerificationToken",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Ward",
                table: "ShippingAddresses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "ShippingAddresses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "ShippingAddresses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "ShippingAddresses",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "ShippingAddresses",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ShippingAddressId",
                table: "ReturnOrders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ReturnOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TotalPrice",
                table: "ReturnOrders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DiscountName",
                table: "Discounts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Discounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscountCode",
                table: "Discounts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Discounts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Discounts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "Categories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AddColumn<Guid>(
                name: "BookId",
                table: "Carts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "BookTitle",
                table: "Books",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AuthorName",
                table: "Authors",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnOrderDetails_ReturnOrderId",
                table: "ReturnOrderDetails",
                column: "ReturnOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OrderId",
                table: "Payments",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_BookId",
                table: "Carts",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Books_BookId",
                table: "Carts",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "BookId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ShippingAddresses_ShippingAddressId",
                table: "Orders",
                column: "ShippingAddressId",
                principalTable: "ShippingAddresses",
                principalColumn: "ShippingAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnOrderDetails_ReturnOrders_ReturnOrderId",
                table: "ReturnOrderDetails",
                column: "ReturnOrderId",
                principalTable: "ReturnOrders",
                principalColumn: "ReturnOrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnOrders_Orders_OrderId",
                table: "ReturnOrders",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingAddresses_Users_UserId",
                table: "ShippingAddresses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Books_BookId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ShippingAddresses_ShippingAddressId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Orders_OrderId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnOrderDetails_ReturnOrders_ReturnOrderId",
                table: "ReturnOrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnOrders_Orders_OrderId",
                table: "ReturnOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_ShippingAddresses_Users_UserId",
                table: "ShippingAddresses");

            migrationBuilder.DropIndex(
                name: "IX_ReturnOrderDetails_ReturnOrderId",
                table: "ReturnOrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_Payments_OrderId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_Carts_BookId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ReturnOrders");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "ReturnOrders");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "DiscountCode",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Discounts");

            migrationBuilder.DropColumn(
                name: "BookId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Carts");

            migrationBuilder.RenameColumn(
                name: "Ward",
                table: "ShippingAddresses",
                newName: "ward");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "ShippingAddresses",
                newName: "country");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "ShippingAddresses",
                newName: "city");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "ReturnOrders",
                newName: "reason");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "OrderDetails",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Discounts",
                newName: "price");

            migrationBuilder.AlterColumn<string>(
                name: "VerificationToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ward",
                table: "ShippingAddresses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "ShippingAddresses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "country",
                table: "ShippingAddresses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "city",
                table: "ShippingAddresses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "ShippingAddresses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<Guid>(
                name: "ShippingAddressId",
                table: "ReturnOrders",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ReturnOrderDetailId",
                table: "ReturnOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentStatus",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Transaction",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderDetailId",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DiscountName",
                table: "Discounts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "Categories",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "BookTitle",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "AuthorName",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateTable(
                name: "CartDetails",
                columns: table => new
                {
                    CartDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CartId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartDetails", x => x.CartDetailId);
                    table.ForeignKey(
                        name: "FK_CartDetails_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "BookId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartDetails_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "CartId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReturnOrders_ReturnOrderDetailId",
                table: "ReturnOrders",
                column: "ReturnOrderDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderDetailId",
                table: "Orders",
                column: "OrderDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetails_BookId",
                table: "CartDetails",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetails_CartId",
                table: "CartDetails",
                column: "CartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_OrderDetails_OrderDetailId",
                table: "Orders",
                column: "OrderDetailId",
                principalTable: "OrderDetails",
                principalColumn: "OrderDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ShippingAddresses_ShippingAddressId",
                table: "Orders",
                column: "ShippingAddressId",
                principalTable: "ShippingAddresses",
                principalColumn: "ShippingAddressId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Users_UserId",
                table: "Orders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Orders_PaymentId",
                table: "Payments",
                column: "PaymentId",
                principalTable: "Orders",
                principalColumn: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnOrders_Orders_OrderId",
                table: "ReturnOrders",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnOrders_ReturnOrderDetails_ReturnOrderDetailId",
                table: "ReturnOrders",
                column: "ReturnOrderDetailId",
                principalTable: "ReturnOrderDetails",
                principalColumn: "ReturnOrderDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_UserId",
                table: "Reviews",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingAddresses_Users_UserId",
                table: "ShippingAddresses",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
