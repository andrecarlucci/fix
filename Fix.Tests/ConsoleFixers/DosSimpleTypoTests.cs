using Fix.CommandFixers;
using Fix.ConsoleHelpers;
using Fix.Tests;
using System;
using Xunit;

namespace Fix.ConsoleFixers.Tests
{
    public class DosSimpleTypoTests
    {
        [Fact]
        public void Can_fix_simple_dos_typo__dir()
        {
            var consoleBuffer = @"C:\dev\app>dri /a /b /o
'dri' is not recognized as an internal or external command,
operable program or batch file.

C:\dev\app>fix
";
            var lines = consoleBuffer.Split(Environment.NewLine);
            ConsoleHelper.GetCurrentPath = () => @"C:\dev\app";

            var manager = SetupTestsHelper.CreateActionManager();
            var fix = manager.GetFix(lines);

            Assert.Equal(nameof(DosSimpleTypo), fix.Author);
            Assert.True(fix.IsFixed);
            Assert.Equal("dir /a /b /o", fix.FixedCommand);
        }
    }
}