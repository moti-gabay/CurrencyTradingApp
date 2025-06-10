using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataLayer.Migrations
{
    /// <inheritdoc />
    public partial class InitialSeedDataStatic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyPairs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OriginalId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PairName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BaseCurrencyAbbr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuoteCurrencyAbbr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InitialRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Volume = table.Column<long>(type: "bigint", nullable: false),
                    LastUpdate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyPairs", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Abbreviation", "Country", "Name" },
                values: new object[,]
                {
                    { 1, "USD", "United States", "Dollar" },
                    { 2, "EUR", "Europe", "Euro" },
                    { 3, "JPY", "Japan", "Yen" },
                    { 4, "GBP", "United Kingdom", "Pound Sterling" },
                    { 5, "CAD", "Canada", "Canadian Dollar" },
                    { 6, "AUD", "Australia", "Australian Dollar" }
                });

            migrationBuilder.InsertData(
                table: "CurrencyPairs",
                columns: new[] { "Id", "BaseCurrencyAbbr", "CurrentRate", "InitialRate", "LastUpdate", "MaxValue", "MinValue", "OriginalId", "PairName", "QuoteCurrencyAbbr", "Volume" },
                values: new object[,]
                {
                    { new Guid("a0000000-0000-0000-0000-000000000001"), "USD", 151.3377m, 151.3377m, new DateTime(2024, 6, 10, 10, 0, 0, 0, DateTimeKind.Utc), 151.3466m, 148.7500m, "USDJPY", "USD/JPY", "JPY", 2225354L },
                    { new Guid("a0000000-0000-0000-0000-000000000002"), "USD", 0.8292m, 0.8292m, new DateTime(2024, 6, 10, 10, 0, 0, 0, DateTimeKind.Utc), 0.8456m, 0.8291m, "USDEUR", "USD/EUR", "EUR", 1370298L },
                    { new Guid("a0000000-0000-0000-0000-000000000003"), "GBP", 1.2485m, 1.2485m, new DateTime(2024, 6, 10, 10, 0, 0, 0, DateTimeKind.Utc), 1.2678m, 1.2485m, "GBPUSD", "GBP/USD", "USD", 1007790L },
                    { new Guid("a0000000-0000-0000-0000-000000000004"), "CAD", 0.7334m, 0.7334m, new DateTime(2024, 6, 10, 10, 0, 0, 0, DateTimeKind.Utc), 0.7350m, 0.7250m, "CADUSD", "CAD/USD", "USD", 606143L },
                    { new Guid("a0000000-0000-0000-0000-000000000005"), "EUR", 162.7026m, 162.7026m, new DateTime(2024, 6, 10, 10, 0, 0, 0, DateTimeKind.Utc), 163.5000m, 162.0000m, "EURJPY", "EUR/JPY", "JPY", 507156L },
                    { new Guid("a0000000-0000-0000-0000-000000000006"), "AUD", 0.6624m, 0.6624m, new DateTime(2024, 6, 10, 10, 0, 0, 0, DateTimeKind.Utc), 0.6700m, 0.6600m, "AUDUSD", "AUD/USD", "USD", 490210L }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "CurrencyPairs");
        }
    }
}
