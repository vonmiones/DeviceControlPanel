using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceControlPanel.Classes
{
    public static class ConversionUtils
    {
        public static int ToInt(this string value)
        {
            int result;
            if (int.TryParse(value, out result))
                return result;
            else
                throw new ArgumentException("Failed to convert string to int.");
        }

        public static float ToFloat(this string value)
        {
            float result;
            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                return result;
            else
                throw new ArgumentException("Failed to convert string to float.");
        }

        public static decimal ToDecimal(this string value)
        {
            decimal result;
            if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                return result;
            else
                throw new ArgumentException("Failed to convert string to decimal.");
        }

        public static bool ToBool(this string value)
        {
            bool result;
            if (bool.TryParse(value, out result))
                return result;
            else
                throw new ArgumentException("Failed to convert string to bool.");
        }

        public static string ToIntString(this string value, int paddingWidth)
        {
            int intValue;
            if (int.TryParse(value, out intValue))
                return intValue.ToString($"D{paddingWidth}");
            else
                throw new ArgumentException("Failed to convert string to int.");
        }
    }
}
