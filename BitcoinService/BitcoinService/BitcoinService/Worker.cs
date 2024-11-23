using System.Net.Http;
using System.Text.Json;

namespace BitcoinService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly HttpClient _httpClient;

        private decimal _totalPriceUsd = 0;
        private int _fetchCounter = 0;


        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    await GetBitcoinRateAsync();

                    if (_fetchCounter >= 2)
                    {
                        LogAverage();
                    }
                }
                await Task.Delay(60000, stoppingToken);
            }
        }

        private async Task GetBitcoinRateAsync()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("https://api.coingecko.com/api/v3/simple/price?ids=bitcoin&vs_currencies=usd");
                var resObj = JsonSerializer.Deserialize<BitcoinPriceResponse>(response);

                var priceUsd = resObj?.bitcoin?.usd ?? 0;

                _totalPriceUsd += priceUsd;
                _fetchCounter++;

                _logger.LogInformation($"The Bitcoin Price at {DateTimeOffset.Now} is: ${priceUsd}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error fetching bitcoin data: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error: {ex.Message}");
            }
        }

        private void LogAverage()
        {
            decimal avg = _fetchCounter != 0 ? _totalPriceUsd / _fetchCounter : 0;
            _logger.LogInformation($"Average Bitcoin price for the last 10 minutes is: {avg}");
            _fetchCounter = 0;
            _totalPriceUsd = 0;
        }
    }

    public class BitcoinPriceResponse
    {
        public BitcoinData? bitcoin { get; set; }
    }
    public class BitcoinData
    {
        public decimal usd { get; set; }
    }
}
