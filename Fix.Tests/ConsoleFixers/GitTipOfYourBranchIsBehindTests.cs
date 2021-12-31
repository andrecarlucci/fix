using Fix.CommandFixers;
using Fix.ConsoleHelpers;
using Fix.Tests;
using System;
using Xunit;

namespace Fix.ConsoleFixers.Tests
{
    public class GitTipOfYourBranchIsBehindTests
    {
        [Fact]
        public void Can_fix_git_tip_of_your_branch_is_behind()
        {
            var consoleBuffer = @"C:\dev\app>git push --set-upstream origin main
To https://github.com/some-app/some-repo.git
 ! [rejected]        main -> main (non-fast-forward)
error: failed to push some refs to 'https://github.com/some-app/some-repo'
hint: Updates were rejected because the tip of your current branch is behind
hint: its remote counterpart. Integrate the remote changes (e.g.
hint: 'git pull ...') before pushing again.
hint: See the 'Note about fast-forwards' in 'git push --help' for details.
C:\dev\app>fix
";
            var lines = consoleBuffer.Split(Environment.NewLine);
            ConsoleHelper.GetCurrentPath = () => @"C:\dev\app";

            var manager = SetupTestsHelper.CreateActionManager();
            var fix = manager.GetFix(lines);

            Assert.Equal(nameof(GitTipOfYourBranchIsBehind), fix.Author);
            Assert.True(fix.IsFixed);
            Assert.Equal("git pull origin main", fix.FixedCommand);
        }
    }
}