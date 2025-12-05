using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public static class NumberToWords
    {
        private static readonly string[] Units = { "", "Một", "Hai", "Ba", "Bốn", "Năm", "Sáu", "Bảy", "Tám", "Chín" };
        private static readonly string[] Tens = { "", "Mười", "Hai mươi", "Ba mươi", "Bốn mươi", "Năm mươi", "Sáu mươi", "Bảy mươi", "Tám mươi", "Chín mươi" };
        private static readonly string[] Hundreds = { "", "Một trăm", "Hai trăm", "Ba trăm", "Bốn trăm", "Năm trăm", "Sáu trăm", "Bảy trăm", "Tám trăm", "Chín trăm" };

        private static readonly string[] BigUnits = { "", "Ngàn", "Triệu", "Tỷ" };

        public static string ConvertAmountToWords(decimal amount)
        {
            long integerPart = (long)amount;
            string integerPartInWords = ConvertIntegerToWords(integerPart);

            long fractionPart = (long)((amount - integerPart) * 100);
            string fractionPartInWords = fractionPart > 0 ? "và " + ConvertIntegerToWords(fractionPart) + " xu" : "";

            return $"{integerPartInWords} đồng {fractionPartInWords}";
        }

        private static string ConvertIntegerToWords(long number)
        {
            if (number == 0)
            {
                return "Không";
            }

            string words = "";
            int bigUnitIndex = 0;

            while (number > 0)
            {
                long part = number % 1000;
                if (part > 0)
                {
                    string partInWords = ConvertThreeDigitNumberToWords(part);
                    words = partInWords + (bigUnitIndex > 0 ? " " + BigUnits[bigUnitIndex] : "") + " " + words;
                }
                number /= 1000;
                bigUnitIndex++;
            }

            return words.Trim();
        }

        private static string ConvertThreeDigitNumberToWords(long number)
        {
            int hundreds = (int)(number / 100);
            int tens = (int)((number % 100) / 10);
            int units = (int)(number % 10);

            string words = "";

            if (hundreds > 0)
            {
                words += Hundreds[hundreds] + " ";
            }

            if (tens > 1)
            {
                words += Tens[tens] + " ";
                if (units > 0)
                {
                    words += Units[units];
                }
            }
            else if (tens == 1)
            {
                words += "Mười ";
                if (units > 1)
                {
                    words += Units[units];
                }
            }
            else if (units > 0)
            {
                words += Units[units];
            }

            return words.Trim();
        }
    }

}
