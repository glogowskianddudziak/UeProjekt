using System;
using UeProject.Helpers.CustomExceptions;

namespace UeProject.Helpers
{
    public static class ModelsHelper
    {
        public static bool ValidateConvertGetModel(string from, decimal amount, string to)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to) || amount <= 0)
                return false;
            return true;
        }
    }
}