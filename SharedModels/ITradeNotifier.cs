// SharedModels/ITradeNotifier.cs
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharedModels
{
    public interface ITradeNotifier
    {
        // *** תיקון: שנה את שם המתודה ל-NotifyCurrencyPairUpdate
        // *** ותן לפרמטר שם הגיוני יותר, כמו currencyPairs, כדי שיתאים ל-DTOs
        Task NotifyCurrencyPairUpdate(List<CurrencyPairDto> currencyPairs);

        // *** תיקון: שנה את שם המתודה ל-NotifyDashboardData, כדי שיתאים לשם המקובל ב-TradeNotifier
        // *** וודא ששם הפרמטר הוא dashboardData
        Task NotifyDashboardData(DashboardData dashboardData);
    }
}