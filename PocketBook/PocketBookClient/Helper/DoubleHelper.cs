using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PocketBookClient.Helper
{
    public static class DoubleHelper
    {
        private static int RoundDigital = 2;
        private static int CompareRoundDigital = 5;
        public static double Round(double value)
        {
            return double.Round(value, RoundDigital);
        }

        private static double RoundInternal(double value)
        {
            return double.Round(value,CompareRoundDigital);
        }

        public static bool AlmostGreat(this double value1,double value2) 
        {
            return RoundInternal(value1) > RoundInternal(value2);
        }
        public static bool AlmostGreatorEqual(this double value1, double value2)
        {
            return RoundInternal(value1) >= RoundInternal(value2);
        }

        public static bool AlmostLess(this double value1, double value2)
        {
            return RoundInternal(value1) < RoundInternal(value2);
        }
        public static bool AlmostLessorEqual(this double value1, double value2)
        {
            return RoundInternal(value1) <= RoundInternal(value2);
        }
    }
}
