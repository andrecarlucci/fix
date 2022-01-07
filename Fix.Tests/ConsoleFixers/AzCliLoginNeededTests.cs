using Fix.CommandFixers;
using Fix.ConsoleHelpers;
using Fix.Tests;
using System;
using Xunit;

namespace Fix.ConsoleFixers.Tests
{
    public class AzCliLoginNeededTests
    {
        [Fact]
        public void Can_fix_azure_cli_login_needed_command()
        {
            var consoleBuffer = @"C:\dev\app>az vm list
User someuser@someemail.com does not exist in MSAL token cache. Run `az login`.

C:\dev\app>fix
";
            var lines = consoleBuffer.Split(Environment.NewLine);
            ConsoleHelper.GetCurrentPath = () => @"C:\dev\app";

            var manager = SetupTestsHelper.CreateActionManager();
            var fix = manager.GetFix(lines);

            Assert.Equal(nameof(AzCliLoginNeeded), fix.Author);
            Assert.True(fix.IsFixed);
            Assert.Equal("az login", fix.FixedCommand);
        }
    }
}