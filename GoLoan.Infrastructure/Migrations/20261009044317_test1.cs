using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoLoan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class test1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DisbursementId",
                table: "LoanAccounts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_LoanAccounts_DisbursementId",
                table: "LoanAccounts",
                column: "DisbursementId");

            migrationBuilder.AddForeignKey(
                name: "FK_LoanAccounts_Disbursements_DisbursementId",
                table: "LoanAccounts",
                column: "DisbursementId",
                principalTable: "Disbursements",
                principalColumn: "DisbursementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoanAccounts_Disbursements_DisbursementId",
                table: "LoanAccounts");

            migrationBuilder.DropIndex(
                name: "IX_LoanAccounts_DisbursementId",
                table: "LoanAccounts");

            migrationBuilder.DropColumn(
                name: "DisbursementId",
                table: "LoanAccounts");
        }
    }
}
