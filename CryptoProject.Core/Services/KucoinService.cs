using CryptoProject.Core.DTOs;
using CryptoProject.Core.Exceptions;
using CryptoProject.Core.Interfaces;
using Kucoin.Net.Clients;

namespace CryptoProject.Core.Services
{
    public class KucoinService : IExchangeService
    {
        private readonly KucoinRestClient _restClient;

        public KucoinService(KucoinRestClient restClient)
        {
            _restClient = restClient;
        }

        /// <summary>
        /// Returns the exchange name and the output amount of the cryptocurrency
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Exchange name and output amount of the cryptocurrency</returns>
        /// <exception cref="InvalidInputAmountException"></exception>
        public async Task<EstimateResultDto> GetExchangeEstimate(EstimateRequestDto data)
        {
            if (data.InputAmount is null or <= 0)
            {
                throw new InvalidInputAmountException("Input amount must be greater than 0");
            }

            var rate = await GetRate(data.InputCurrency, data.OutputCurrency);

            var outputAmount = data.InputAmount * rate;

            return new EstimateResultDto
            {
                ExchangeName = ExchangesContants.KUCOIN,
                OutputAmount = outputAmount
            };
        }

        /// <summary>
        /// Returns the exchange name and the price of the cryptocurrency
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="quoteCurrency"></param>
        /// <returns>Exchange name and price of the cryptocurrency</returns>
        public async Task<RateResultDto> GetExchangeRate(string baseCurrency, string quoteCurrency)
        {
            var rate = await GetRate(baseCurrency, quoteCurrency);

            return new RateResultDto
            {
                ExchangeName = ExchangesContants.KUCOIN,
                Rate = rate
            };
        }


        /// <summary>
        /// Returns the price of the specified cryptocurrency on Kucoin exchange
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="quoteCurrency"></param>
        /// <returns>Price of the cryptocurrency</returns>
        /// <summary>
        /// <exception cref="InvalidCurrencyPairException"></exception>
        private async Task<decimal?> GetRate(string baseCurrency, string quoteCurrency)
        {
            if (baseCurrency == quoteCurrency)
            {
                throw new InvalidCurrencyPairException("Base currency and quote currency cannot be the same.");
            }

            decimal? rate;

            var basePair = $"{baseCurrency}-USDT";
            var quotePair = $"{quoteCurrency}-USDT";


            if (baseCurrency == "USDT")
            {
                var quotePrice = await GetLastPrice(quotePair);
                rate = 1 / quotePrice;
            }
            else if (quoteCurrency == "USDT")
            {
                var basePrice = await GetLastPrice(basePair);
                rate = basePrice;
            }
            else
            {
                var basePrice = await GetLastPrice(basePair);
                var quotePrice = await GetLastPrice(quotePair);
                rate = basePrice / quotePrice;
            }

            return rate;
        }

        /// <summary>
        /// Returns the last price of the cryptocurrency
        /// </summary>
        /// <param name="pair"></param>
        /// <returns>Last pair</returns>
        /// <exception cref="ApiException"></exception>
        private async Task<decimal?> GetLastPrice(string pair)
        {
            var tickerResult = await _restClient.SpotApi.ExchangeData.GetTickerAsync(pair);

            if (tickerResult.Data == null)
            {
                throw new ApiException($"Failed to get valid data for {pair}.");
            }

            return tickerResult.Data.LastPrice;
        }
    }
}
