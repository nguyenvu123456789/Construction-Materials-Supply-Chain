using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common
{
    public static class NumberToWords
    {
        private static readonly string[] Units = { "", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
        private static readonly string[] Tens = { "", "mười", "hai mươi", "ba mươi", "bốn mươi", "năm mươi", "sáu mươi", "bảy mươi", "tám mươi", "chín mươi" };
        private static readonly string[] Thousands = { "", "nghìn", "triệu", "tỷ" };

        public static string Convert(decimal number)
        {
            if (number == 0) return "không đồng";

            long intPart = (long)number;
            long decimalPart = (long)((number - intPart) * 100);

            string intPartInWords = ConvertIntegerPart(intPart);
            string decimalPartInWords = decimalPart > 0 ? " và " + ConvertIntegerPart(decimalPart) + " xu" : "";

            return intPartInWords + decimalPartInWords;
        }

        private static string ConvertIntegerPart(long number)
        {
            if (number == 0) return "không";

            string result = "";
            int thousandUnit = 0;

            while (number > 0)
            {
                int part = (int)(number % 1000);
                if (part > 0)
                {
                    result = ConvertHundreds(part) + Thousands[thousandUnit] + " " + result;
                }
                number /= 1000;
                thousandUnit++;
            }

            return result.Trim();
        }

        private static string ConvertHundreds(int number)
        {
            string result = "";

            if (number >= 100)
            {
                result += Units[number / 100] + " trăm ";
                number %= 100;
            }

            if (number >= 20)
            {
                result += Tens[number / 10] + " ";
                number %= 10;
            }
            else if (number >= 10)
            {
                result += "mười ";
                number %= 10;
            }

            if (number > 0)
            {
                result += Units[number] + " ";
            }

            return result.Trim();
        }
    }
}
