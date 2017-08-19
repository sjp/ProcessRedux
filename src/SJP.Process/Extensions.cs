using System;
using System.Collections.Generic;
using System.Text;

namespace SJP.Process
{
    public static class NumericExtensions
    {
        public static int Clamp(this int number, int minValue, int maxValue)
        {
            if (number < minValue)
                return minValue;
            if (number > maxValue)
                return maxValue;

            return number;
        }

        public static double Clamp(this double number, double minValue, double maxValue)
        {
            if (number < minValue)
                return minValue;
            if (number > maxValue)
                return maxValue;

            return number;
        }
    }

    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string input) => string.IsNullOrEmpty(input);

        public static bool IsNullOrWhiteSpace(this string input) => string.IsNullOrWhiteSpace(input);
    }
}
