namespace CryptoProject.Core.DTOs
{
    public record EstimateRequestDto(decimal? InputAmount, string InputCurrency, string OutputCurrency);
}
