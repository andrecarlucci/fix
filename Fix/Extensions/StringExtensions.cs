using System.Text.RegularExpressions;

namespace Fix.Helpers
{
    public static class StringExtensions
    {
        private static Regex _findFirstWordBetweenSingleQuotesRegex = new(@"'([^']*)");

        public static string FindFirstWordBetweenSingleQuotes(this string text)
        {
            var match = _findFirstWordBetweenSingleQuotesRegex.Match(text);
            return match.Success ? match.Groups[1].Value : "";
        }

        public static string ReplaceFirst(this string text, string oldValue, string newValue)
        {
            var regex = new Regex(Regex.Escape(oldValue));
            return regex.Replace(text, newValue, 1);
        }
    }
}