// DataLayer/Models/CurrencyPair.cs
using System;
using SharedModels; // <--- הוסף את זה כדי להכיר את SharedModels.TradeTrend

namespace DataLayer.Models
{
    public class CurrencyPair
    {
        // שינוי ה-Id ל-Guid כדי להתאים ל-DbContext ול-SeedData
        public Guid Id { get; set; }
        public string OriginalId { get; set; }
        public string PairName { get; set; }
        public string BaseCurrencyAbbr { get; set; }
        public string QuoteCurrencyAbbr { get; set; }
        public decimal CurrentRate { get; set; }
        public decimal InitialRate { get; set; } // <--- הוסף את זה, כי הוא מופיע ב-SeedData
        public decimal ChangePercentage { get; set; } // <--- הוסף את זה!
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public long Volume { get; set; }
        public SharedModels.TradeTrend Trend { get; set; } // <--- הוסף את זה! (מפנה ל-SharedModels.TradeTrend)
        public DateTime LastUpdate { get; set; }
    }
}