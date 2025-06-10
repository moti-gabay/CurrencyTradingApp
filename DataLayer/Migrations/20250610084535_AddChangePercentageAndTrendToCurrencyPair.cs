using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddChangePercentageAndTrendToCurrencyPair : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ChangePercentage",
                table: "CurrencyPairs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Trend",
                table: "CurrencyPairs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "CurrencyPairs",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000001"),
                columns: new[] { "ChangePercentage", "Trend" },
                values: new object[] { 0.15m, 0 });

            migrationBuilder.UpdateData(
                table: "CurrencyPairs",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000002"),
                columns: new[] { "ChangePercentage", "Trend" },
                values: new object[] { -0.05m, 1 });

            migrationBuilder.UpdateData(
                table: "CurrencyPairs",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000003"),
                columns: new[] { "ChangePercentage", "Trend" },
                values: new object[] { 0.00m, 2 });

            migrationBuilder.UpdateData(
                table: "CurrencyPairs",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000004"),
                columns: new[] { "ChangePercentage", "Trend" },
                values: new object[] { 0.08m, 0 });

            migrationBuilder.UpdateData(
                table: "CurrencyPairs",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000005"),
                columns: new[] { "ChangePercentage", "Trend" },
                values: new object[] { -0.20m, 1 });

            migrationBuilder.UpdateData(
                table: "CurrencyPairs",
                keyColumn: "Id",
                keyValue: new Guid("a0000000-0000-0000-0000-000000000006"),
                columns: new[] { "ChangePercentage", "Trend" },
                values: new object[] { 0.00m, 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangePercentage",
                table: "CurrencyPairs");

            migrationBuilder.DropColumn(
                name: "Trend",
                table: "CurrencyPairs");
        }
    }
}
