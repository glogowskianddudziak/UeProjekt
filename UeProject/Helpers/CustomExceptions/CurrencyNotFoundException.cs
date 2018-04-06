using System;

namespace UeProject.Helpers.CustomExceptions
{
    public class CurrencyNotFoundException : Exception
    {
        public CurrencyNotFoundException()
        {
            
        }

        public CurrencyNotFoundException(string currency) : base($"Currency {currency} not found")
        {
            
        }
    }
}