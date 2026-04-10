using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fomo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProfitColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Profit",
                table: "TradeResults");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Profit",
                table: "TradeResults",
                type: "numeric(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
