using System.Text;
using System.Text.RegularExpressions;

namespace Savage.PhoneNumber
{
    public class PhoneNumberFormatter
    {
        public PhoneNumberFormatter(string phoneNumber, string defaultCountryCode)
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
            var formatter = new PhoneNumberFormatter(phoneNumber, defaultCountryCode);
            return formatter.ToInternationalFormat();
        }

        public string ToInternationalFormat()
        {
            StringBuilder result = new StringBuilder();

            result.Append($"{CountryCode} ");

            if (AreaCode != string.Empty)
                result.Append($"({AreaCode}) ");

            result.Append(LocalNumber);

            if (Extension != string.Empty)
                result.Append($"x{Extension}");

            return result.ToString();
        }
        
        public string ToDialFormat(string fromCountryCode, string idPrefix, string fromAreaCode, string ndPrefix)
        {
            StringBuilder result = new StringBuilder();

            if (CountryCode != fromCountryCode)
            {
                result.Append($"{CountryCode.Replace("+", idPrefix)} ");
                if (AreaCode != string.Empty)
                {
                    result.Append($"({AreaCode}) ");
                }
            }
            else
            {
                if (AreaCode == string.Empty)
                {
                    result.Append(ndPrefix);
                }
                else if (AreaCode != fromAreaCode)
                {
                    result.Append($"({ndPrefix}{AreaCode}) ");
                }
            }

            result.Append(LocalNumber);

            if (Extension != string.Empty)
                result.Append($"x{Extension}");

            return result.ToString();
        }
    }
}
