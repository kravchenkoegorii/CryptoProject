using CryptoProject.Core.DTOs;
using CryptoProject.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CryptoProject.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExchangeController : ControllerBase
    {
        private readonly IEnumerable<IExchangeService> _exchangeServices;

        public ExchangeController(IEnumerable<IExchangeService> exchangeServices)
        {
            _exchangeServices = exchangeServices;
        }

        [HttpGet("rates")]
        public async Task<List<RateResultDto>> GetAllCurrencyRates(string baseCurrency, string quoteCurrency)
        {
            var tasks = _exchangeServices.Select(service => service.GetExchangeRate(baseCurrency, quoteCurrency));

            var exchangesRate = await Task.WhenAll(tasks);

            var ratesResult = exchangesRate.ToList();

            return ratesResult;
        }

        [HttpPost("estimate")]
        public async Task<EstimateResultDto> GetCurrencyEstimate(EstimateRequestDto data)
        {
            var tasks = _exchangeServices.Select(service => service.GetExchangeEstimate(data));

            var exchangesEstimates = await Task.WhenAll(tasks);

            var estimateResult = exchangesEstimates.MaxBy(result => result.OutputAmount);

            return estimateResult;
        }
    }
}
