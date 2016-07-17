using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugTrackingSystem.Service
{
    public static class ConvertHelper
    {
        public static string ConvertStringArrayToString(string[] arrayToConvert)
        {
            var result = string.Join(",", arrayToConvert);
            return result;
        }

        public static string ConvertIntArrayToString(int[] arrayToConvert)
        {
            var result = string.Join(",", arrayToConvert);
            return result;
        }

        public static string[] ConvertStringToStringArray(string stringToConvert)
        {
            if (string.IsNullOrEmpty(stringToConvert))
                return null;

            var result = stringToConvert.Split(',');
            return result;
        }

        public static int[] ConvertStringToIntArray(string stringToConvert)
        {
            if (string.IsNullOrEmpty(stringToConvert))
                return null;

            var stringArray = stringToConvert.Split(',');
            var result = stringArray.Select(n => Convert.ToInt32(n)).ToArray();
            return result;
        }
    }
}
