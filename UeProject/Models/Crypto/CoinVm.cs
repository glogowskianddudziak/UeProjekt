namespace UeProject.Models.Crypto
{
    public class CoinVm
    {
        public string Label { get; set; }
        public string Name { get; set; }
        public decimal Price_btc { get; set; }
        public decimal Price_cny { get; set; }
        public decimal Price_eur { get; set; }
        public decimal Price_gbp { get; set; }
        public decimal Price_rur { get; set; }
        public decimal Price_usd { get; set; }
        public long Timestamp { get; set; }
        public decimal Volume_24h { get; set; }
    }
}