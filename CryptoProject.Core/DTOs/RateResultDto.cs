namespace CryptoProject.Core.DTOs
{
    public record RateResultDto
    {
        public string ExchangeName { get; set; }

        public decimal? Rate { get; set; }
    }
}
