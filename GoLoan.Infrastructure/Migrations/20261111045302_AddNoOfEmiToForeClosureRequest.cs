using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoLoan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNoOfEmiToForeClosureRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartialAmount",
                table: "ForeClosureRequests");

            migrationBuilder.AddColumn<int>(
                name: "NoOfEmi",
                table: "ForeClosureRequests",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoOfEmi",
                table: "ForeClosureRequests");

            migrationBuilder.AddColumn<decimal>(
                name: "PartialAmount",
                table: "ForeClosureRequests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
