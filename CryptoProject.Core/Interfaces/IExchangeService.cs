using CryptoProject.Core.DTOs;

namespace CryptoProject.Core.Interfaces
{
    public interface IExchangeService
    {
        public Task<RateResultDto> GetExchangeRate(string baseCurrency, string quoteCurrency);

        public Task<EstimateResultDto> GetExchangeEstimate(EstimateRequestDto data);
    }
}
