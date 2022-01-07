using System.Text.RegularExpressions;

namespace Fix.Helpers
{
    public static class StringFinder
    {
        private static Regex _findFirstWordBetweenSingleQuotesRegex = new(@"'([^']*)");

        public static string FindFirstWordBetweenSingleQuotes(string value)
        {
            var match = _findFirstWordBetweenSingleQuotesRegex.Match(value);
            return match.Success ? match.Groups[1].Value : "";
        }

        public static string ReplaceFirst(string text, string oldValue, string newValue)
        {
            var regex = new Regex(Regex.Escape(oldValue));
            return regex.Replace(text, newValue, 1);
        }
    }
}