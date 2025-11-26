using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarWashFinancialSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddExpensesAndFinancialReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Services",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpenseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OperatorId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_Users_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FinancialReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Period = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ServicesCount = table.Column<int>(type: "int", nullable: false),
                    AverageCheck = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WaterExpenses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ElectricityExpenses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HeatingExpenses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalaryExpenses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ChemicalExpenses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RentExpenses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaintenanceExpenses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OtherExpenses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalExpenses = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NetProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProfitMargin = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialReports", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 21, 9, 1, 54, 162, DateTimeKind.Local).AddTicks(5658));

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_OperatorId",
                table: "Expenses",
                column: "OperatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "FinancialReports");

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "Description", "DurationMinutes", "IsActive", "Name", "Price" },
                values: new object[] { 4, "Чистка салона", 60, true, "Химчистка салона", 2000m });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 12, 16, 0, 13, 609, DateTimeKind.Local).AddTicks(5482));
        }
    }
}
