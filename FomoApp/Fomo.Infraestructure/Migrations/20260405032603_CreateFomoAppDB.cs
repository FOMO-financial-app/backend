using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Fomo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateFomoAppDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    StockId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Symbol = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Currency = table.Column<string>(type: "text", nullable: false),
                    Exchange = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.StockId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Auth0Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    SmaAlert = table.Column<bool>(type: "boolean", nullable: false),
                    BollingerAlert = table.Column<bool>(type: "boolean", nullable: false),
                    StochasticAlert = table.Column<bool>(type: "boolean", nullable: false),
                    RsiAlert = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "TradeResults",
                columns: table => new
                {
                    TradeResultId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Symbol = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    EntryPrice = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    ExitPrice = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    Profit = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    NumberOfStocks = table.Column<int>(type: "integer", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExitDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeResults", x => x.TradeResultId);
                    table.CheckConstraint("CK_Product_EntryPrice_NonNegative", "\"EntryPrice\" >= 0");
                    table.CheckConstraint("CK_Product_ExitPrice_NonNegative", "\"ExitPrice\" >= 0");
                    table.CheckConstraint("CK_Product_NumberOfStocks_NonNegative", "\"NumberOfStocks\" >= 0");
                    table.ForeignKey(
                        name: "FK_TradeResults_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TradeMethods",
                columns: table => new
                {
                    TradeMethodId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Sma = table.Column<bool>(type: "boolean", nullable: false),
                    Bollinger = table.Column<bool>(type: "boolean", nullable: false),
                    Stochastic = table.Column<bool>(type: "boolean", nullable: false),
                    Rsi = table.Column<bool>(type: "boolean", nullable: false),
                    Other = table.Column<bool>(type: "boolean", nullable: false),
                    TradeResultId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradeMethods", x => x.TradeMethodId);
                    table.ForeignKey(
                        name: "FK_TradeMethods_TradeResults_TradeResultId",
                        column: x => x.TradeResultId,
                        principalTable: "TradeResults",
                        principalColumn: "TradeResultId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_Name",
                table: "Stocks",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_Symbol",
                table: "Stocks",
                column: "Symbol");

            migrationBuilder.CreateIndex(
                name: "IX_TradeMethods_TradeResultId",
                table: "TradeMethods",
                column: "TradeResultId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TradeResults_UserId",
                table: "TradeResults",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Auth0Id",
                table: "Users",
                column: "Auth0Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "TradeMethods");

            migrationBuilder.DropTable(
                name: "TradeResults");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
