using Binance.Net.Clients;
using CryptoProject.Core.Interfaces;
using CryptoProject.Core.Services;
using Kucoin.Net.Clients;

namespace CryptoProject.Core.DependencyInjection
{
    public static class ExchangeDependencyInjection
    {
        public static IServiceCollection AddExchangeServices(this IServiceCollection services)
        {
            services.AddScoped<BinanceRestClient>();
            services.AddScoped<KucoinRestClient>();

            services.AddScoped<IExchangeService, BinanceService>();
            services.AddScoped<IExchangeService, KucoinService>();

            return services;
        }
    }
}
