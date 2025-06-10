// BusinessLayer/SimulationService.cs
using DataLayer.Repositories;
using Microsoft.Extensions.Hosting;
using SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection; // חובה! עבור CreateScope ו-GetRequiredService
using Microsoft.Extensions.Logging;
using DataLayer.Models;

namespace BusinessLayer
{
    public class SimulationService : IHostedService, ISimulationService, IDisposable
    {
        private Timer _timer;
        private readonly IServiceProvider _serviceProvider; // זה מה שנזריק
        private readonly ILogger<SimulationService> _logger;
        // *** REMOVE THIS LINE: private readonly ITradeNotifier _tradeNotifier; ***

        private bool _isSimulationRunning = false;
        private Random _random = new Random();

        private CancellationTokenSource _simulationCts;
        private List<CurrencyPairDto> _currentCurrencyPairs;
        private DashboardData _currentDashboardData;

        public SimulationService(IServiceProvider serviceProvider,
                                 ILogger<SimulationService> logger
                                 // *** REMOVE THIS PARAMETER: , ITradeNotifier tradeNotifier ***
                                 )
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            // *** REMOVE THIS LINE: _tradeNotifier = tradeNotifier; ***
            _logger.LogInformation("SimulationService initialized.");

            _currentCurrencyPairs = GetInitialCurrencyPairs();
            _currentDashboardData = GetInitialDashboardData(_currentCurrencyPairs);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SimulationService (IHostedService) running. Timer will start when StartSimulation() is called from API.");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SimulationService (IHostedService) stopping.");
            StopSimulation();
            return Task.CompletedTask;
        }

        public void StartSimulation()
        {
            if (!_isSimulationRunning)
            {
                _logger.LogInformation("SimulationService: API requested simulation start. Starting timer.");
                _simulationCts = new CancellationTokenSource();
                _timer = new Timer(async (state) => await DoWork(state), null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
                _isSimulationRunning = true;
            }
            else
            {
                _logger.LogInformation("SimulationService: Simulation already running.");
            }
        }

        public void StopSimulation()
        {
            if (_isSimulationRunning)
            {
                _logger.LogInformation("SimulationService: API requested simulation stop. Stopping timer.");
                _timer?.Change(Timeout.Infinite, 0);
                _simulationCts?.Cancel();
                _simulationCts?.Dispose();
                _isSimulationRunning = false;
            }
            else
            {
                _logger.LogInformation("SimulationService: Simulation not running, cannot stop.");
            }
        }

        public DashboardData GetCurrentDashboardData()
        {
            UpdateSimulationData(_currentCurrencyPairs, _currentDashboardData);
            return _currentDashboardData;
        }

        private async Task DoWork(object state)
        {
            _logger.LogDebug("SimulationService: DoWork started.");

            // *** זהו ה-Scope שפותר את הבעיה! ***
            using (var scope = _serviceProvider.CreateScope())
            {
                var currencyPairRepository = scope.ServiceProvider.GetRequiredService<ICurrencyPairRepository>();
                // *** הוסף שורה זו: קבל את ה-ITradeNotifier מתוך ה-Scope הנוכחי ***
                var tradeNotifier = scope.ServiceProvider.GetRequiredService<ITradeNotifier>();

                try
                {
                    UpdateSimulationData(_currentCurrencyPairs, _currentDashboardData);

                    var dbCurrencyPairs = _currentCurrencyPairs.Select(dto => new DataLayer.Models.CurrencyPair
                    {
                        Id = dto.Id,
                        OriginalId = dto.OriginalId,
                        PairName = dto.PairName,
                        BaseCurrencyAbbr = dto.BaseCurrencyAbbr,
                        QuoteCurrencyAbbr = dto.QuoteCurrencyAbbr,
                        CurrentRate = dto.CurrentRate,
                        InitialRate = dto.CurrentRate,
                        ChangePercentage = dto.ChangePercentage,
                        MinValue = dto.MinValue,
                        MaxValue = dto.MaxValue,
                        Volume = dto.Volume,
                        Trend = dto.Trend,
                        LastUpdate = dto.LastUpdate
                    }).ToList();

                    await currencyPairRepository.UpdateCurrencyPairsAsync(dbCurrencyPairs);
                    _logger.LogInformation("SimulationService: Currency pairs updated in repository.");

                    // *** השתמש במשתנה tradeNotifier שקיבלת מה-Scope ***
                    await tradeNotifier.NotifyCurrencyPairUpdate(_currentCurrencyPairs);
                    _logger.LogDebug("SimulationService: Notified currency update.");

                    await tradeNotifier.NotifyDashboardData(_currentDashboardData);
                    _logger.LogDebug("SimulationService: Notified dashboard data update.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "SimulationService: An error occurred during simulation work.");
                }
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _simulationCts?.Dispose();
            _logger.LogInformation("SimulationService disposed.");
        }

        private List<CurrencyPairDto> GetInitialCurrencyPairs()
        {
            return new List<CurrencyPairDto>
            {
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000001"), OriginalId = "USDJPY", PairName = "US Dollar / Japanese Yen", BaseCurrencyAbbr = "USD", QuoteCurrencyAbbr = "JPY", CurrentRate = 151.3377m, ChangePercentage = 0.15m, MinValue = 151.0000m, MaxValue = 151.5000m, Volume = 150000000, Trend = SharedModels.TradeTrend.Up, LastUpdate = DateTime.Now },
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000002"), OriginalId = "EURUSD", PairName = "Euro / US Dollar", BaseCurrencyAbbr = "EUR", QuoteCurrencyAbbr = "USD", CurrentRate = 1.0850m, ChangePercentage = -0.05m, MinValue = 1.0800m, MaxValue = 1.0900m, Volume = 200000000, Trend = SharedModels.TradeTrend.Down, LastUpdate = DateTime.Now },
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000003"), OriginalId = "GBPUSD", PairName = "British Pound / US Dollar", BaseCurrencyAbbr = "GBP", QuoteCurrencyAbbr = "USD", CurrentRate = 1.2750m, ChangePercentage = 0.00m, MinValue = 1.2700m, MaxValue = 1.2800m, Volume = 80000000, Trend = SharedModels.TradeTrend.Stable, LastUpdate = DateTime.Now },
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000004"), OriginalId = "AUDUSD", PairName = "Australian Dollar / US Dollar", BaseCurrencyAbbr = "AUD", QuoteCurrencyAbbr = "USD", CurrentRate = 0.6650m, ChangePercentage = 0.08m, MinValue = 0.6600m, MaxValue = 0.6700m, Volume = 70000000, Trend = SharedModels.TradeTrend.Up, LastUpdate = DateTime.Now },
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000005"), OriginalId = "NZDUSD", PairName = "New Zealand Dollar / US Dollar", BaseCurrencyAbbr = "NZD", QuoteCurrencyAbbr = "USD", CurrentRate = 0.6150m, ChangePercentage = -0.10m, MinValue = 0.6100m, MaxValue = 0.6200m, Volume = 40000000, Trend = SharedModels.TradeTrend.Down, LastUpdate = DateTime.Now },
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000006"), OriginalId = "USDCAD", PairName = "US Dollar / Canadian Dollar", BaseCurrencyAbbr = "USD", QuoteCurrencyAbbr = "CAD", CurrentRate = 1.3700m, ChangePercentage = 0.02m, MinValue = 1.3650m, MaxValue = 1.3750m, Volume = 100000000, Trend = SharedModels.TradeTrend.Stable, LastUpdate = DateTime.Now },
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000007"), OriginalId = "USDCHF", PairName = "US Dollar / Swiss Franc", BaseCurrencyAbbr = "USD", QuoteCurrencyAbbr = "CHF", CurrentRate = 0.9050m, ChangePercentage = -0.03m, MinValue = 0.9000m, MaxValue = 0.9100m, Volume = 50000000, Trend = SharedModels.TradeTrend.Down, LastUpdate = DateTime.Now },
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000008"), OriginalId = "EURJPY", PairName = "Euro / Japanese Yen", BaseCurrencyAbbr = "EUR", QuoteCurrencyAbbr = "JPY", CurrentRate = 164.0000m, ChangePercentage = 0.20m, MinValue = 163.5000m, MaxValue = 164.5000m, Volume = 90000000, Trend = SharedModels.TradeTrend.Up, LastUpdate = DateTime.Now },
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000009"), OriginalId = "GBPJPY", PairName = "British Pound / Japanese Yen", BaseCurrencyAbbr = "GBP", QuoteCurrencyAbbr = "JPY", CurrentRate = 192.5000m, ChangePercentage = 0.10m, MinValue = 192.0000m, MaxValue = 193.0000m, Volume = 60000000, Trend = SharedModels.TradeTrend.Up, LastUpdate = DateTime.Now },
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000010"), OriginalId = "EURGBP", PairName = "Euro / British Pound", BaseCurrencyAbbr = "EUR", QuoteCurrencyAbbr = "GBP", CurrentRate = 0.8500m, ChangePercentage = -0.01m, MinValue = 0.8480m, MaxValue = 0.8520m, Volume = 30000000, Trend = SharedModels.TradeTrend.Stable, LastUpdate = DateTime.Now },
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000011"), OriginalId = "USDMEX", PairName = "US Dollar / Mexican Peso", BaseCurrencyAbbr = "USD", QuoteCurrencyAbbr = "MEX", CurrentRate = 17.0000m, ChangePercentage = 0.05m, MinValue = 16.9000m, MaxValue = 17.1000m, Volume = 25000000, Trend = SharedModels.TradeTrend.Up, LastUpdate = DateTime.Now },
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000012"), OriginalId = "USDCNY", PairName = "US Dollar / Chinese Yuan", BaseCurrencyAbbr = "USD", QuoteCurrencyAbbr = "CNY", CurrentRate = 7.2500m, ChangePercentage = -0.02m, MinValue = 7.2400m, MaxValue = 7.2600m, Volume = 15000000, Trend = SharedModels.TradeTrend.Down, LastUpdate = DateTime.Now },
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000013"), OriginalId = "USDKRW", PairName = "US Dollar / South Korean Won", BaseCurrencyAbbr = "USD", QuoteCurrencyAbbr = "KRW", CurrentRate = 1350.00m, ChangePercentage = 0.07m, MinValue = 1340.00m, MaxValue = 1360.00m, Volume = 12000000, Trend = SharedModels.TradeTrend.Up, LastUpdate = DateTime.Now },
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000014"), OriginalId = "USDINR", PairName = "US Dollar / Indian Rupee", BaseCurrencyAbbr = "USD", QuoteCurrencyAbbr = "INR", CurrentRate = 83.5000m, ChangePercentage = 0.03m, MinValue = 83.4000m, MaxValue = 83.6000m, Volume = 10000000, Trend = SharedModels.TradeTrend.Stable, LastUpdate = DateTime.Now },
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000015"), OriginalId = "USDZAR", PairName = "US Dollar / South African Rand", BaseCurrencyAbbr = "USD", QuoteCurrencyAbbr = "ZAR", CurrentRate = 18.5000m, ChangePercentage = -0.08m, MinValue = 18.4000m, MaxValue = 18.6000m, Volume = 8000000, Trend = SharedModels.TradeTrend.Down, LastUpdate = DateTime.Now },
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000016"), OriginalId = "USDRUB", PairName = "US Dollar / Russian Ruble", BaseCurrencyAbbr = "USD", QuoteCurrencyAbbr = "RUB", CurrentRate = 90.0000m, ChangePercentage = 0.00m, MinValue = 89.5000m, MaxValue = 90.5000m, Volume = 7000000, Trend = SharedModels.TradeTrend.Stable, LastUpdate = DateTime.Now },
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000017"), OriginalId = "USDBRL", PairName = "US Dollar / Brazilian Real", BaseCurrencyAbbr = "USD", QuoteCurrencyAbbr = "BRL", CurrentRate = 5.2000m, ChangePercentage = 0.06m, MinValue = 5.1500m, MaxValue = 5.2500m, Volume = 6000000, Trend = SharedModels.TradeTrend.Up, LastUpdate = DateTime.Now },
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000018"), OriginalId = "USDPLN", PairName = "US Dollar / Polish Zloty", BaseCurrencyAbbr = "USD", QuoteCurrencyAbbr = "PLN", CurrentRate = 4.0000m, ChangePercentage = -0.04m, MinValue = 3.9800m, MaxValue = 4.0200m, Volume = 5000000, Trend = SharedModels.TradeTrend.Down, LastUpdate = DateTime.Now },
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000019"), OriginalId = "USDHUF", PairName = "US Dollar / Hungarian Forint", BaseCurrencyAbbr = "USD", QuoteCurrencyAbbr = "HUF", CurrentRate = 365.0000m, ChangePercentage = 0.01m, MinValue = 360.0000m, MaxValue = 370.0000m, Volume = 4000000, Trend = SharedModels.TradeTrend.Stable, LastUpdate = DateTime.Now },
                new CurrencyPairDto { Id = Guid.Parse("A0000000-0000-0000-0000-000000000020"), OriginalId = "USDSEK", PairName = "US Dollar / Swedish Krona", BaseCurrencyAbbr = "USD", QuoteCurrencyAbbr = "SEK", CurrentRate = 10.5000m, ChangePercentage = 0.09m, MinValue = 10.4000m, MaxValue = 10.6000m, Volume = 3000000, Trend = SharedModels.TradeTrend.Up, LastUpdate = DateTime.Now }
            };
        }

        private DashboardData GetInitialDashboardData(List<CurrencyPairDto> pairs)
        {
            return new DashboardData
            {
                CurrencyPairs = pairs,
                TotalVolume = pairs.Sum(p => p.Volume),
                AverageChangePercentage = (decimal)pairs.Average(p => (double)p.ChangePercentage),
                TotalActivePairs = pairs.Count,
                LastUpdated = DateTime.Now
            };
        }

        private void UpdateSimulationData(List<CurrencyPairDto> pairs, DashboardData dashboardData)
        {
            foreach (var pair in pairs)
            {
                double rateChange = (_random.NextDouble() * 0.005) - 0.0025;
                pair.CurrentRate += (decimal)rateChange;

                // Ensure MinValue and MaxValue are updated correctly based on CurrentRate
                if (pair.CurrentRate < pair.MinValue) pair.MinValue = pair.CurrentRate;
                if (pair.CurrentRate > pair.MaxValue) pair.MaxValue = pair.CurrentRate;

                decimal newChangePercentage = (decimal)((_random.NextDouble() * 0.5) - 0.25);
                pair.ChangePercentage = newChangePercentage;

                pair.Volume += _random.Next(50000, 500000);

                pair.LastUpdate = DateTime.Now;

                if (pair.ChangePercentage > 0.1m) pair.Trend = SharedModels.TradeTrend.Up;
                else if (pair.ChangePercentage < -0.1m) pair.Trend = SharedModels.TradeTrend.Down;
                else pair.Trend = SharedModels.TradeTrend.Stable;
            }

            dashboardData.TotalVolume = pairs.Sum(p => p.Volume);
            dashboardData.AverageChangePercentage = (decimal)pairs.Average(p => (double)p.ChangePercentage);
            dashboardData.TotalActivePairs = pairs.Count;
            dashboardData.LastUpdated = DateTime.Now;
        }
    }
}