using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoLoan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmiScheduleIdToLoanPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmiScheduleId",
                table: "LoanPayments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LoanPayments_EmiScheduleId",
                table: "LoanPayments",
                column: "EmiScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanPayments_EmiSchedules_EmiScheduleId",
                table: "LoanPayments",
                column: "EmiScheduleId",
                principalTable: "EmiSchedules",
                principalColumn: "EmiScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanPayments_EmiSchedules_EmiScheduleId",
                table: "LoanPayments");

            migrationBuilder.DropIndex(
                name: "IX_LoanPayments_EmiScheduleId",
                table: "LoanPayments");

            migrationBuilder.DropColumn(
                name: "EmiScheduleId",
                table: "LoanPayments");
        }
    }
}
