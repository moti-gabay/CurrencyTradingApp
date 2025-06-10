// BusinessLayer/DashboardService.cs
using DataLayer.Repositories; // גישה ל-Repository
using SharedModels; // גישה ל-DTOs (עבור CurrencyPairDto, DashboardData, TradeTrend)
using System.Collections.Generic;
using System.Linq; // עבור Sum, Average, Any
using System.Threading.Tasks;
using System; // עבור DateTime.UtcNow

namespace BusinessLayer
{
    // ודא שהמחלקה הזו מממשת את IDashboardService (אם יש לך כזה)
    public class DashboardService : IDashboardService // השאר IDashboardService אם הוא מוגדר
    {
        private readonly ICurrencyPairRepository _currencyPairRepository;
        // כרגע אין צורך ב-ITradeNotifier ב-DashboardService עצמו,
        // אלא אם כן DashboardService אחראי גם לשלוח עדכונים ל-UI באופן ישיר.
        // אם SimulationService הוא זה ששולח, אז לא צריך כאן.
        // private readonly ITradeNotifier _tradeNotifier;

        public DashboardService(ICurrencyPairRepository currencyPairRepository)
        {
            _currencyPairRepository = currencyPairRepository;
            // אם היה לך ITradeNotifier בקונסטרקטור, וודא שאתה מטפל בו בהתאם
            // this._tradeNotifier = tradeNotifier;
        }

        public async Task<DashboardData> GetDashboardDataAsync()
        {
            var currencyPairs = await _currencyPairRepository.GetAllCurrencyPairsAsync();

            var currencyPairDtos = new List<CurrencyPairDto>();
            foreach (var pair in currencyPairs)
            {
                // וודא ש-InitialRate אינו אפס כדי למנוע חלוקה באפס
                var changePercentage = pair.InitialRate != 0 ? (pair.CurrentRate - pair.InitialRate) / pair.InitialRate * 100 : 0m;
                var trend = GetTradeTrend(changePercentage);

                currencyPairDtos.Add(new CurrencyPairDto
                {
                    Id = pair.Id,
                    OriginalId = pair.OriginalId,
                    PairName = pair.PairName,
                    BaseCurrencyAbbr = pair.BaseCurrencyAbbr,
                    QuoteCurrencyAbbr = pair.QuoteCurrencyAbbr,
                    CurrentRate = pair.CurrentRate,
                    InitialRate = pair.InitialRate,
                    ChangePercentage = changePercentage,
                    MinValue = pair.MinValue,
                    MaxValue = pair.MaxValue,
                    Volume = pair.Volume,
                    Trend = trend,
                    LastUpdate = pair.LastUpdate
                });
            }

            // --- זהו החלק שמתקן את השגיאה שהצגת ---
            // חישוב נתוני הסיכום ישירות כאן, ללא קריאה למתודה נפרדת (כמו GetDashboardSummaryAsync)
            var totalActivePairs = currencyPairDtos.Count;
            var totalVolume = currencyPairDtos.Sum(dto => dto.Volume);
            var averageChangePercentage = currencyPairDtos.Any() ?
                currencyPairDtos.Average(dto => dto.ChangePercentage) : 0m;
            var lastUpdated = DateTime.UtcNow;

            // יצירת אובייקט DashboardData יחיד עם כל הנתונים, ללא מאפיין Summary
            return new DashboardData
            {
                CurrencyPairs = currencyPairDtos, // רשימת זוגות המטבעות
                TotalActivePairs = totalActivePairs, // נתוני סיכום שהועברו ישירות ל-DashboardData
                TotalVolume = totalVolume,
                AverageChangePercentage = averageChangePercentage,
                LastUpdated = lastUpdated
            };
            // --- סוף התיקון ---
        }

        // --- המתודה GetDashboardSummaryAsync() נמחקה מכאן לחלוטין. ---
        // היא אינה קיימת יותר, והלוגיקה שלה מוזגה לתוך GetDashboardDataAsync().

        private TradeTrend GetTradeTrend(decimal changePercentage)
        {
            if (changePercentage > 0.1m) return TradeTrend.Up;
            if (changePercentage < -0.1m) return TradeTrend.Down;
            return TradeTrend.Stable;
        }
    }
}