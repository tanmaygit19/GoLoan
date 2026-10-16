using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoLoan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class adddata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RazorpayAmount",
                table: "EmiSchedules",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RazorpayPenalty",
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
                name: "RazorpayAmount",
                table: "EmiSchedules");

            migrationBuilder.DropColumn(
                name: "RazorpayPenalty",
                table: "EmiSchedules");
        }
    }
}
