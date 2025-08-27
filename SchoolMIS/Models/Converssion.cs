using System;
using System.Collections.Generic;
using  System.Globalization;
using System.Linq;
using System.Web;

namespace SchoolMIS.Models
{
    public static class Converssion
    {
        public static string ConvertEasternArabicToWestern(string input)
        {
            char[] arabicDigits = { '\u0660', '\u0661', '\u0662', '\u0663', '\u0664', '\u0665', '\u0666', '\u0667', '\u0668', '\u0669' };
            char[] persianDigits = { '\u06F0', '\u06F1', '\u06F2', '\u06F3', '\u06F4', '\u06F5', '\u06F6', '\u06F7', '\u06F8', '\u06F9' };

            for (int i = 0; i < 10; i++)
            {
                input = input.Replace(arabicDigits[i], (char)('0' + i));
                input = input.Replace(persianDigits[i], (char)('0' + i));
            }

            return input;
        }



        public static string ToPersianDateString(string input, bool convertSlash = true)
        {
            var westernDigits = "0123456789";
            var persianDigits = "۰۱۲۳۴۵۶۷۸۹";

            var result = new System.Text.StringBuilder();

            foreach (var ch in input)
            {
                if (char.IsDigit(ch))
                {
                    int index = westernDigits.IndexOf(ch);
                    result.Append(persianDigits[index]);
                }
                else if (ch == '/' && convertSlash)
                {
                    result.Append('/'); // You can also use '/' if you prefer
                }
                else
                {
                    result.Append(ch);
                }
            }

            return result.ToString();
        }









        public static string ToShamsi(DateTime date)
        {
            PersianCalendar pc = new PersianCalendar();
            int year = pc.GetYear(date);
            int month = pc.GetMonth(date);
            int day = pc.GetDayOfMonth(date);

            return $"{year:0000}/{month:00}/{day:00}";
        }
        public static DateTime ConvertShamsiToGregorian(string shamsiDate)
        {

            try
            {
                PersianCalendar pc = new PersianCalendar();
                // Ensure standardized format
                var parts = shamsiDate.Trim().Replace('-', '/').Split('/');
                if (parts.Length != 3)
                    throw new FormatException("Invalid Shamsi date format");

                int year = int.Parse(parts[0]);
                int month = int.Parse(parts[1]);
                int day = int.Parse(parts[2]);


                return pc.ToDateTime(year, month, day, 0, 0, 0, 0);
            }
            catch (Exception ex)
            {
                // Optional: return default date or log error
                throw new Exception("Error converting Shamsi to Gregorian: " + ex.Message);
            }
        }
    }


   

}