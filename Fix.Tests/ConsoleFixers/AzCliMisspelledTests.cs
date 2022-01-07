using Fix.CommandFixers;
using Fix.ConsoleHelpers;
using Fix.Tests;
using System;
using Xunit;

namespace Fix.ConsoleFixers.Tests
{
    public class AzCliMisspelledTests
    {
        [Fact]
        public void Can_fix_azure_cli_misspelled_command()
        {
            var consoleBuffer = @"C:\dev\app>az upgradr
'upgradr' is misspelled or not recognized by the system.
Did you mean 'upgrade' ?

Examples from AI knowledge base:
https://aka.ms/cli_ref
Read more about the command in reference docs

C:\dev\app>fix
";
            var lines = consoleBuffer.Split(Environment.NewLine);
            ConsoleHelper.GetCurrentPath = () => @"C:\dev\app";

            var manager = SetupTestsHelper.CreateActionManager();
            var fix = manager.GetFix(lines);

            Assert.Equal(nameof(AzCliMisspelled), fix.Author);
            Assert.True(fix.IsFixed);
            Assert.Equal("az upgrade", fix.FixedCommand);
        }
    }
}