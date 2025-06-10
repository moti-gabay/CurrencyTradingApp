// DataLayer/Data/ApplicationDbContext.cs
using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using SharedModels; // <--- הוסף את זה כדי להכיר את SharedModels.TradeTrend

namespace DataLayer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CurrencyPair> CurrencyPairs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // אין צורך בקשרי גומלין מפורשים בין CurrencyPair ל-Currency
            // מכיוון שאנו משתמשים בקיצורים (Abbreviation) ולא ב-Foreign Keys.

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var usd = new Currency { Id = 1, Country = "United States", Name = "Dollar", Abbreviation = "USD" };
            var eur = new Currency { Id = 2, Country = "Europe", Name = "Euro", Abbreviation = "EUR" };
            var jpy = new Currency { Id = 3, Country = "Japan", Name = "Yen", Abbreviation = "JPY" };
            var gbp = new Currency { Id = 4, Country = "United Kingdom", Name = "Pound Sterling", Abbreviation = "GBP" };
            var cad = new Currency { Id = 5, Country = "Canada", Name = "Canadian Dollar", Abbreviation = "CAD" };
            var aud = new Currency { Id = 6, Country = "Australia", Name = "Australian Dollar", Abbreviation = "AUD" };

            modelBuilder.Entity<Currency>().HasData(usd, eur, jpy, gbp, cad, aud);

            modelBuilder.Entity<CurrencyPair>().HasData(
                new CurrencyPair
                {
                    Id = Guid.Parse("A0000000-0000-0000-0000-000000000001"),
                    OriginalId = "USDJPY",
                    PairName = "USD/JPY",
                    BaseCurrencyAbbr = "USD",
                    QuoteCurrencyAbbr = "JPY",
                    CurrentRate = 151.3377m,
                    InitialRate = 151.3377m, // <--- וודא שזה קיים וממופה
                    ChangePercentage = 0.15m, // <--- הוסף את זה!
                    MinValue = 148.7500m,
                    MaxValue = 151.3466m,
                    Volume = 2225354,
                    Trend = SharedModels.TradeTrend.Up, // <--- הוסף את זה!
                    LastUpdate = new DateTime(2024, 6, 10, 10, 0, 0, DateTimeKind.Utc)
                },
                new CurrencyPair
                {
                    Id = Guid.Parse("A0000000-0000-0000-0000-000000000002"),
                    OriginalId = "USDEUR",
                    PairName = "USD/EUR",
                    BaseCurrencyAbbr = "USD",
                    QuoteCurrencyAbbr = "EUR",
                    CurrentRate = 0.8292m,
                    InitialRate = 0.8292m, // <--- וודא שזה קיים וממופה
                    ChangePercentage = -0.05m, // <--- הוסף את זה!
                    MinValue = 0.8291m,
                    MaxValue = 0.8456m,
                    Volume = 1370298,
                    Trend = SharedModels.TradeTrend.Down, // <--- הוסף את זה!
                    LastUpdate = new DateTime(2024, 6, 10, 10, 0, 0, DateTimeKind.Utc)
                },
                new CurrencyPair
                {
                    Id = Guid.Parse("A0000000-0000-0000-0000-000000000003"),
                    OriginalId = "GBPUSD",
                    PairName = "GBP/USD",
                    BaseCurrencyAbbr = "GBP",
                    QuoteCurrencyAbbr = "USD",
                    CurrentRate = 1.2485m,
                    InitialRate = 1.2485m, // <--- וודא שזה קיים וממופה
                    ChangePercentage = 0.00m, // <--- הוסף את זה!
                    MinValue = 1.2485m,
                    MaxValue = 1.2678m,
                    Volume = 1007790,
                    Trend = SharedModels.TradeTrend.Stable, // <--- הוסף את זה!
                    LastUpdate = new DateTime(2024, 6, 10, 10, 0, 0, DateTimeKind.Utc)
                },
                new CurrencyPair
                {
                    Id = Guid.Parse("A0000000-0000-0000-0000-000000000004"),
                    OriginalId = "CADUSD",
                    PairName = "CAD/USD",
                    BaseCurrencyAbbr = "CAD",
                    QuoteCurrencyAbbr = "USD",
                    CurrentRate = 0.7334m,
                    InitialRate = 0.7334m, // <--- וודא שזה קיים וממופה
                    ChangePercentage = 0.08m, // <--- הוסף את זה!
                    MinValue = 0.7250m,
                    MaxValue = 0.7350m,
                    Volume = 606143,
                    Trend = SharedModels.TradeTrend.Up, // <--- הוסף את זה!
                    LastUpdate = new DateTime(2024, 6, 10, 10, 0, 0, DateTimeKind.Utc)
                },
                new CurrencyPair
                {
                    Id = Guid.Parse("A0000000-0000-0000-0000-000000000005"),
                    OriginalId = "EURJPY",
                    PairName = "EUR/JPY",
                    BaseCurrencyAbbr = "EUR",
                    QuoteCurrencyAbbr = "JPY",
                    CurrentRate = 162.7026m,
                    InitialRate = 162.7026m, // <--- וודא שזה קיים וממופה
                    ChangePercentage = -0.20m, // <--- הוסף את זה!
                    MinValue = 162.0000m,
                    MaxValue = 163.5000m,
                    Volume = 507156,
                    Trend = SharedModels.TradeTrend.Down, // <--- הוסף את זה!
                    LastUpdate = new DateTime(2024, 6, 10, 10, 0, 0, DateTimeKind.Utc)
                },
                new CurrencyPair
                {
                    Id = Guid.Parse("A0000000-0000-0000-0000-000000000006"),
                    OriginalId = "AUDUSD",
                    PairName = "AUD/USD",
                    BaseCurrencyAbbr = "AUD",
                    QuoteCurrencyAbbr = "USD",
                    CurrentRate = 0.6624m,
                    InitialRate = 0.6624m, // <--- וודא שזה קיים וממופה
                    ChangePercentage = 0.00m, // <--- הוסף את זה!
                    MinValue = 0.6600m,
                    MaxValue = 0.6700m,
                    Volume = 490210,
                    Trend = SharedModels.TradeTrend.Stable, // <--- הוסף את זה!
                    LastUpdate = new DateTime(2024, 6, 10, 10, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}