using System;

namespace UeProject.Helpers
{
    public class ExchangeLogic : IExchangeLogic
    {
        public decimal ConvertCryptoCurrency(decimal fromUsdRate, decimal toUsdRate, decimal amount)
        {
            if(fromUsdRate <= 0 || toUsdRate <= 0)
                throw new Exception();
            
            return amount * fromUsdRate / toUsdRate;
        }

        public decimal ConvertCryptoCurrencyToDollars(decimal fromUsdRate, decimal amount)
        {
            if (fromUsdRate <= 0 || amount <= 0)
                throw new Exception();

            return amount * fromUsdRate;
        }
    }
}