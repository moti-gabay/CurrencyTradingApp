// UILayer/Hubs/TradeHubNotifier.cs
using Microsoft.AspNetCore.SignalR;
using SharedModels; // חובה! עבור ITradeNotifier, ITradeClient, CurrencyPairDto, DashboardData
using System.Collections.Generic; // עבור List
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System; // עבור Exception

namespace UILayer.Hubs // זהו ה-Namespace שבו נמצא ה-Notifier שלך
{
    // זו המחלקה שמממשת את ITradeNotifier
    public class TradeHubNotifier : ITradeNotifier
    {
        // IHubContext צריך להכיר את ה-Hub (TradeHub) ואת ה-Client Interface (ITradeClient)
        private readonly IHubContext<TradeHub, ITradeClient> _hubContext;
        private readonly ILogger<TradeHubNotifier> _logger;

        public TradeHubNotifier(IHubContext<TradeHub, ITradeClient> hubContext, ILogger<TradeHubNotifier> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        // *** תיקון 1: שינוי שם המתודה מ-NotifyCurrencyUpdate ל-NotifyCurrencyPairUpdate
        // *** ושינוי שם הפרמטר מ-tradeData ל-currencyPairs (לשם עקביות ובהירות)
        public async Task NotifyCurrencyPairUpdate(List<CurrencyPairDto> currencyPairs)
        {
            _logger.LogDebug($"TradeHubNotifier: Sending NotifyCurrencyPairUpdate for {currencyPairs.Count} items.");
            try
            {
                // שם המתודה בצד הקליינט. וודא שהקליינט מצפה ל-List<CurrencyPairDto>
                await _hubContext.Clients.All.ReceiveCurrencyPairUpdate(currencyPairs);
                _logger.LogDebug($"TradeHubNotifier: Successfully sent NotifyCurrencyPairUpdate.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"TradeHubNotifier: Error sending NotifyCurrencyPairUpdate.");
            }
        }

        // *** תיקון 2: שינוי שם המתודה מ-NotifyDashboardDataUpdate ל-NotifyDashboardData
        // *** ושינוי שם הפרמטר מ-summaryData ל-dashboardData (לשם עקביות ובהירות)
        public async Task NotifyDashboardData(DashboardData dashboardData) // <--- פרמטר הטיפוס נשאר DashboardData
        {
            _logger.LogDebug($"TradeHubNotifier: Sending NotifyDashboardData. Total Volume: {dashboardData.TotalVolume}.");

            try
            {
                // וודא ש-ReceiveDashboardData ב-ITradeClient מקבל DashboardData
                await _hubContext.Clients.All.ReceiveDashboardData(dashboardData);
                _logger.LogDebug($"TradeHubNotifier: Successfully sent NotifyDashboardData.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TradeHubNotifier: Error sending NotifyDashboardData.");
            }
        }
    }
}