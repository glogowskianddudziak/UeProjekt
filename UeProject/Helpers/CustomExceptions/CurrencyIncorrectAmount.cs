using System;

namespace UeProject.Helpers.CustomExceptions
{
    public class CurrencyIncorrectAmount : Exception
    {
        public CurrencyIncorrectAmount() : base("Amount cannot be 0 or negative")
        {
            
        }
    }
}