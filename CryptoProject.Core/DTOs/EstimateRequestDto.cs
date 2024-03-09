namespace CryptoProject.Core.DTOs
{
    public class EstimateRequestDto
    {
        public decimal? InputAmount { get; set; }

        public string InputCurrency { get; set; }

        public string OutputCurrency { get; set; }
    }
}
