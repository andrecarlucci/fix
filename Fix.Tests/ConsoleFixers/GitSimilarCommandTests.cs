using Fix.CommandFixers;
using System;
using Xunit;
using Fix.Tests;

namespace Fix.ConsoleFixers.Tests
{
    public class GitSimilarCommandTests
    {
        [Fact]
        public void Can_fix_git_status()
        {
            var consoleBuffer = @"C:\dev\app>git stats
git: 'stats' is not a git command. See 'git --help'.

The most similar command is
        status
C:\dev\app>fix
";
            var lines = consoleBuffer.Split(Environment.NewLine);

            var manager = SetupTestsHelper.CreateActionManager();
            var fix = manager.GetFix(lines);

            Assert.True(fix.IsFixed);
            Assert.Equal(nameof(GitSimilar), fix.Author);
            Assert.Equal("git status", fix.FixedCommand);
        }

        [Fact]
        public void Can_fix_git_status_from_windows_terminal()
        {
            var consoleBuffer = @"PS C:\dev\app> git pu
git: 'pu' is not a git command. See 'git --help'.

The most similar command is
        pull
        push
        p4
PS C:\dev\app> fix
";
            var lines = consoleBuffer.Split(Environment.NewLine);

            var manager = SetupTestsHelper.CreateActionManager();
            var fix = manager.GetFix(lines);

            Assert.True(fix.IsFixed);
            Assert.Equal(nameof(GitSimilar), fix.Author);
            Assert.Equal("git pull", fix.FixedCommand);
        }
    }
}