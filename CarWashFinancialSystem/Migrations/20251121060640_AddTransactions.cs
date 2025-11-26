using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarWashFinancialSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 21, 9, 6, 40, 33, DateTimeKind.Local).AddTicks(270));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 21, 9, 1, 54, 162, DateTimeKind.Local).AddTicks(5658));
        }
    }
}
