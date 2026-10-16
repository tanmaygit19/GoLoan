using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoLoan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class adddata2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BonusPaid",
                table: "LoanPayments",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "RazorpayBonus",
                table: "EmiSchedules",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BonusPaid",
                table: "LoanPayments");

            migrationBuilder.DropColumn(
                name: "RazorpayBonus",
                table: "EmiSchedules");
        }
    }
}
