namespace UeProject.Helpers
{
    public interface IExchangeLogic
    {
        decimal ConvertCryptoCurrency(decimal fromUsdRate, decimal toUsdRate, decimal amount);

        decimal ConvertCryptoCurrencyToDollars(decimal fromUsdRate, decimal amount);
    }
}
