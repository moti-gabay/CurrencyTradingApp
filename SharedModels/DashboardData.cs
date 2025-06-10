// SharedModels/DashboardData.cs
using System.Collections.Generic; // חובה!
using System; // חובה עבור DateTime

namespace SharedModels
{
    public class DashboardData // וודא שזה public class
    {
        // נתוני זוגות המטבעות
        public List<CurrencyPairDto> CurrencyPairs { get; set; } = new List<CurrencyPairDto>();

        // המאפיינים שהיו ב-DashboardSummary, עברו לכאן ישירות
        public int TotalActivePairs { get; set; }
        public long TotalVolume { get; set; }
        public decimal AverageChangePercentage { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}