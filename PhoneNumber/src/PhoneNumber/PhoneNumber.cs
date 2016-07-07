using System;
using System.Text.RegularExpressions;
using System.Text;

namespace Savage.Formatters
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class PhoneNumber
    {
        public PhoneNumber(string phoneNumber, string defaultCountryCode)
        {
            defaultCountryCode = Regex.Replace(defaultCountryCode, "^00", "+");
            Input = phoneNumber.ToUpperInvariant().Trim();
            //Check if the phone number contains a country code
            string match = GetSegment(@"^(\+|00)(?'number'[0-9]{1,3})");

            CountryCode = string.IsNullOrEmpty(match) ? defaultCountryCode : string.Format("+{0}", match);
        }

        private string Input { get; }
        public string CountryCode { get; }

        private string GetSegment(string pattern)
        {
            Match match = Regex.Match(Input, pattern);

            if (match.Groups["number"] != null)
                return match.Groups["number"].Value;
            else
                return "";
        }

        public string AreaCode => GetSegment(@"(^|\s|\()0?(?'number'[1-9][0-9]{1,4})(\s|\))");

        public string LocalNumber => Regex.Replace(GetSegment(@"(^|\s|\))0?(?'number'[0-9\s]{4,10})(X|EXT|$)"), @"\s", "");

        public string Extension => GetSegment(@"(X|EXT)\D*(?'number'[0-9]{1,5})");

        public static string ToInternationalFormat(string phoneNumber, string defaultCountryCode)
        {
            PhoneNumber utility = new PhoneNumber(phoneNumber, defaultCountryCode);
            return utility.ToInternationalFormat();
        }

        public string ToInternationalFormat()
        {
            StringBuilder result = new StringBuilder();

            result.Append(String.Format("{0} ", CountryCode));

            if (AreaCode != String.Empty)
                result.Append(String.Format("({0}) ", AreaCode));

            result.Append(LocalNumber);

            if (Extension != String.Empty)
                result.Append(String.Format("x{0}", Extension));

            return result.ToString();
        }

        public string ToDialFormat(string fromCountryCode, string idPrefix, string fromAreaCode, string ndPrefix)
        {
            StringBuilder result = new StringBuilder();

            if (CountryCode != fromCountryCode)
            {
                result.Append(String.Format("{0} ", CountryCode.Replace("+", idPrefix)));
                if (AreaCode != String.Empty)
                {
                    result.Append(String.Format("({0}) ", AreaCode));
                }
            }
            else
            {
                if (AreaCode == String.Empty)
                {
                    result.Append(ndPrefix);
                }
                else if (AreaCode != fromAreaCode)
                {
                    result.Append(String.Format("({0}{1}) ", ndPrefix, AreaCode));
                }
            }

            result.Append(LocalNumber);

            if (Extension != String.Empty)
                result.Append(String.Format("x{0}", Extension));

            return result.ToString();
        }
    }
}
