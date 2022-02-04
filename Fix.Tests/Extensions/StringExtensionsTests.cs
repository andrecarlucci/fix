using Fix.Helpers;
using Xunit;

namespace Fix.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("cmd", "cmd fo bar", "cmd foo bar")]
        [InlineData("cmd foo", "cmd foo bar", "cmd foo bar1")]
        [InlineData("cmd foo bar", "cmd foo bar", "cmd foo bar")]
        [InlineData("", "cmd1 foo bar", "cmd foo bar")]
        public void Test(string expected, string one, string two)
        {
            var result = one.FindCommonStartingWords(two);
            Assert.Equal(expected, result);
        }
    }
}
