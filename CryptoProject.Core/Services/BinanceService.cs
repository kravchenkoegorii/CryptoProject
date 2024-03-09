using Binance.Net.Clients;
using Binance.Net.Objects.Models.Spot;
using CryptoProject.Core.DTOs;
using CryptoProject.Core.Exceptions;
using CryptoProject.Core.Interfaces;

namespace CryptoProject.Core.Services
{
    public class BinanceService : IExchangeService
    {
        private readonly BinanceRestClient _restClient;

        public BinanceService(BinanceRestClient restClient)
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

            return new EstimateResultDto(ExchangesContants.BINANCE, outputAmount);
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

            return new RateResultDto(ExchangesContants.BINANCE, rate);
        }

        /// <summary>
        /// Returns the price of the specified cryptocurrency on Binance exchange
        /// </summary>
        /// <param name="baseCurrency"></param>
        /// <param name="quoteCurrency"></param>
        /// <returns>Price of the cryptocurrency</returns>
        private async Task<decimal?> GetRate(string baseCurrency, string quoteCurrency)
        {
            if (baseCurrency == quoteCurrency)
            {
                throw new InvalidCurrencyPairException("Base currency and quote currency cannot be the same.");
            }

            decimal? rate = null;

            var basePair = $"{baseCurrency}USDT";
            var quotePair = $"{quoteCurrency}USDT";

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
                var pricesList = await GetLastPrices([basePair, quotePair]);

                var basePrice = pricesList.FirstOrDefault(price => price.Symbol == basePair)?.Price;
                var quotePrice = pricesList.FirstOrDefault(price => price.Symbol == quotePair)?.Price;

                rate = basePrice / quotePrice;
            }

            return rate;
        }

        /// <summary>
        /// Returns the last price of the cryptocurrency
        /// </summary>
        /// <param name="pair"></param>
        /// <returns>Last price of the pair</returns>
        /// <exception cref="ApiException"></exception>
        private async Task<decimal?> GetLastPrice(string pair)
        {
            var pairData = await _restClient.SpotApi.ExchangeData.GetPriceAsync(pair);

            if (pairData.Data == null)
            {
                throw new ApiException($"Failed to get valid data for {pair}.");
            }

            return pairData.Data.Price;
        }

        /// <summary>
        /// Returns the last prices of the cryptocurrencies
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns>Last price of the pairs</returns>
        /// <exception cref="ApiException"></exception>
        private async Task<List<BinancePrice>> GetLastPrices(string[] pairs)
        {
            var pairsData = await _restClient.SpotApi.ExchangeData.GetPricesAsync(pairs);

            if (pairsData.Data == null)
            {
                throw new ApiException($"Failed to get valid data for several pairs.");
            }

            return pairsData.Data.ToList();
        }
    }
}
