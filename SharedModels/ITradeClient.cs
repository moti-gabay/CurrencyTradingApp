// SharedModels/ITradeClient.cs
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharedModels
{
    // זהו הממשק שהלקוחות (פרונט-אנד) יממשו/ישתמשו בו
    public interface ITradeClient
    {
        Task ReceiveCurrencyPairUpdate(List<CurrencyPairDto> currencyPairs);
        Task ReceiveDashboardData(DashboardData dashboardData);
        // הוסף כאן כל פונקציה נוספת שהשרת קורא ללקוח
    }
}