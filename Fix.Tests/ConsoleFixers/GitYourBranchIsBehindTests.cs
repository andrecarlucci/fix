using Fix.CommandFixers;
using Fix.ConsoleHelpers;
using Fix.Tests;
using System;
using Xunit;

namespace Fix.ConsoleFixers.Tests
{
    public class GitYourBranchIsBehindTests
    {
        [Fact]
        public void Can_fix_git_your_branch_is_behind()
        {
            var consoleBuffer = @"C:\dev\app>git push origin main
To https://github.com/some-user/some-repo
 ! [rejected]        main -> main (fetch first)
error: failed to push some refs to 'https://github.com/some-user/some-repo'
hint: Updates were rejected because the remote contains work that you do
hint: not have locally. This is usually caused by another repository pushing
hint: to the same ref. You may want to first integrate the remote changes
hint: (e.g., 'git pull ...') before pushing again.
hint: See the 'Note about fast-forwards' in 'git push --help' for details.
C:\dev\app>fix
";
            var lines = consoleBuffer.Split(Environment.NewLine);
            ConsoleHelper.GetCurrentPath = () => @"C:\dev\app";

            var manager = SetupTestsHelper.CreateActionManager();
            var fix = manager.GetFix(lines);

            Assert.Equal(nameof(GitYourBranchIsBehind), fix.Author);
            Assert.True(fix.IsFixed);
            Assert.Equal("git pull origin main", fix.FixedCommand);
        }
    }
}