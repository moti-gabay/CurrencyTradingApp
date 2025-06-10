// SharedModels/CurrencyPairDto.cs
using System;

namespace SharedModels
{
    public class CurrencyPairDto
    {
        // שינוי ה-Id ל-Guid כדי להתאים למודל ה-DB
        public Guid Id { get; set; }
        public string OriginalId { get; set; }
        public string PairName { get; set; }
        public string BaseCurrencyAbbr { get; set; }
        public string QuoteCurrencyAbbr { get; set; }
        public decimal CurrentRate { get; set; }
        public decimal InitialRate { get; set; }
        public decimal ChangePercentage { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public long Volume { get; set; }
        public TradeTrend Trend { get; set; } // כאן ה-Enum TradeTrend הוא מתוך SharedModels
        public DateTime LastUpdate { get; set; }
    }
}