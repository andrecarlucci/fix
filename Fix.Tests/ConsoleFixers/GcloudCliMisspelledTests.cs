using Fix.CommandFixers;
using Fix.ConsoleHelpers;
using Fix.Tests;
using System;
using Xunit;

namespace Fix.ConsoleFixers.Tests
{
    public class GcloudCliMisspelledTests
    {
        [Fact]
        public void Can_fix_gcloud_cli_misspelled_commands()
        {
            var consoleBuffer = @"C:\dev\app>gcloud project list
ERROR: (gcloud) Invalid choice: 'project'.
Maybe you meant:
  gcloud projects

To search the help text of gcloud commands, run:
  gcloud help -- SEARCH_TERMS

C:\dev\app>fix
";
            var lines = consoleBuffer.Split(Environment.NewLine);
            ConsoleHelper.GetCurrentPath = () => @"C:\dev\app";

            var manager = SetupTestsHelper.CreateActionManager();
            var fix = manager.GetFix(lines);

            Assert.Equal(nameof(GcloudCliMisspelled), fix.Author);
            Assert.True(fix.IsFixed);
            Assert.Equal("gcloud projects list", fix.FixedCommand);
        }
    }
}