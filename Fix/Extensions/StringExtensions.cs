using System.IO.Enumeration;
using System.Linq;
using System.Text;
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

        /// <summary>
        /// Matches string using * on left or right
        /// </summary>
        /// <param name="text"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool MatchesSimpleExpression(this string text, string expression)
        {
            return FileSystemName.MatchesSimpleExpression(expression, text);
        }

        public static string FindCommonStartingWords(this string one, string two)
        {
            var sb = new StringBuilder();
            var v1 = one.Split(' ');
            var v2 = two.Split(' ');

            for (var i = 0; i < v1.Length; i++)
            {
                if(v2.Length < i)
                {
                    break;
                }
                if(v1[i] == v2[i])
                {
                    if(sb.Length > 0)
                    {
                        sb.Append(' ');
                    }
                    sb.Append(v1[i]);
                }
                else
                {
                    break;
                }
            }
            return sb.ToString();
        }

        public static string GetWord(this string value, int index) => value.Split(' ')[index];
        public static string GetLastWord(this string value) => value.Split(' ').Last();
    }
}