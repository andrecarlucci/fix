using Fix.CommandFixers;
using Fix.ConsoleHelpers;
using System;
using Xunit;

namespace Fix.Tests
{
    public class GitBranchHasNoUpstreamTests
    {
        [Fact]
        public void Can_fix_git_branch_no_upstream()
        {
            var consoleBuffer = @"C:\dev\app>git push
fatal: The current branch main has no upstream branch.
To push the current branch and set the remote as upstream, use

    git push --set-upstream origin main


C:\dev\app>fix
";
            var lines = consoleBuffer.Split(Environment.NewLine);
            ConsoleHelper.GetCurrentPath = () => @"C:\dev\app";

            var manager = ActionManagerHelper.CreateActionManager();
            var fix = manager.GetFix(lines);

            Assert.Equal(nameof(GitBranchHasNoUpstream), fix.Author);
            Assert.True(fix.IsFixed);
            Assert.Equal("git push --set-upstream origin main", fix.FixedCommand);
        }
    }
}