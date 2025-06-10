// UILayer/Services/TradeNotifier.cs
using BusinessLayer; // עבור ISignalRNotifier
using Microsoft.AspNetCore.SignalR;
using SharedModels; // <--- זה קריטי! עבור ITradeClient, CurrencyPairDto, DashboardData
using UILayer.Hubs; // עבור TradeHub (ממחלקת ה-Hub הספציפית)
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // עבור ILogger

namespace UILayer.Services
{
    public class TradeNotifier : ITradeNotifier
    {
        // *** התיקון הקריטי כאן: IHubContext צריך את מחלקת ה-HUB הספציפית (TradeHub)
        // ואת ממשק הלקוח (ITradeClient) כפי שהוא מוגדר ב-SharedModels. ***
        private readonly IHubContext<TradeHub, ITradeClient> _hubContext;
        private readonly ILogger<TradeNotifier> _logger; // שילוב ה-logger בקונסטרוקטור

        public TradeNotifier(IHubContext<TradeHub, ITradeClient> hubContext, ILogger<TradeNotifier> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
            _logger.LogInformation("TradeNotifier initialized.");
        }

        public async Task NotifyCurrencyPairUpdate(List<CurrencyPairDto> currencyPairs)
        {
            try
            {
                await _hubContext.Clients.All.ReceiveCurrencyPairUpdate(currencyPairs);
                _logger.LogDebug("TradeNotifier: Sent currency pair update to all clients.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TradeNotifier: Error sending currency pair update.");
            }
        }

        public async Task NotifyDashboardData(DashboardData dashboardData)
        {
            try
            {
                await _hubContext.Clients.All.ReceiveDashboardData(dashboardData);
                _logger.LogDebug("TradeNotifier: Sent dashboard data to all clients.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TradeNotifier: Error sending dashboard data.");
            }
        }
    }
}