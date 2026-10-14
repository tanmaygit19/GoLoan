using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoLoan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class testing1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoanType",
                table: "SanctionLetters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoanType",
                table: "SanctionLetters");
        }
    }
}
