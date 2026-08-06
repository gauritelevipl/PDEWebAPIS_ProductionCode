using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PDEWebAPIS.CommonMethods
{
    public class CommonFunctions
    {
        public string ConvertStringToDate(string Inputdate)
        {
            if (DateTime.TryParse(Inputdate, out DateTime date))
            {
                // Format to 'yyyy-MM-dd'
                string formattedDate = date.ToString("yyyy-MM-dd");
                return formattedDate;
            }
            else
            {
                return "Invalid date format.";
            }
            //if (DateTime.TryParseExact(Inputdate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            //{
            //    string formattedDate = date.ToString("yyyy-MM-dd");
            //    return formattedDate;
            //}
            //else
            //{
            //    return "Invalid date format.";
            //}
        }
        
        public string ReplaceNA(string namePart)
        {
            if (namePart == null)
            {
                namePart = "NA";
            }
            return namePart.Equals("NA", StringComparison.OrdinalIgnoreCase) ? string.Empty : namePart.Trim();
        }


        public bool IsEnglishNumber(string input)
        {
            foreach (char c in input)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return input.Length > 0;
        }

        public bool IsEnglishNumber(char c) => c >= '0' && c <= '9';

        public bool IsForwardSlashOrComma(char c) => c == '/' || c == ',';

        public bool IsDotOrComma(char c) => c == '.' || c == ',';

        public bool IsEnglishLetter(char c) =>
            (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');

        public bool IsMarathiLetter(char c) =>
            // Devanagari letters: U+0900 to U+097F
            c >= '\u0900' && c <= '\u097F';

        public bool IsMarathiNumber(char c) =>
            // Devanagari digits: U+0966 (०) to U+096F (९)
            c >= '\u0966' && c <= '\u096F';

        public bool IsSpace(char c) => c == ' ';

        public bool CheckDastNabhuNo(string input)
        {
            foreach (char c in input)
            {
                if (IsEnglishNumber(c) ||
                 IsForwardSlashOrComma(c) ||
                 IsEnglishLetter(c) ||
                 IsMarathiLetter(c) ||
                 IsMarathiNumber(c) ||
                  IsDotOrComma(c) ||
                 IsSpace(c))
                {
                    continue;
                }
                else
                {
                    return false; // found invalid char
                }
            }
            return true; // all good
        }

        public bool CheckDastRemark(string input)
        {
            string pattern = @"^[0-9A-Za-z\u0900-\u097F.,\s]+$";
            bool isValid = Regex.IsMatch(input, pattern);
            foreach (char c in input)
            {
                if (IsMarathiNumber(c))
                {
                    isValid = false;
                    break;
                }
            }
            return isValid;
        }


    }
}
