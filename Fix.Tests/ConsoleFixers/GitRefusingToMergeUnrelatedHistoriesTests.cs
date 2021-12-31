using Fix.CommandFixers;
using Fix.ConsoleHelpers;
using Fix.Tests;
using System;
using Xunit;

namespace Fix.ConsoleFixers.Tests
{
    public class GitRefusingToMergeUnrelatedHistoriesTests
    {
        [Fact]
        public void Can_fix_Git_Refusing_To_Merge_Unrelated_HistoriesTest()
        {
            var consoleBuffer = @"C:\dev\app>git pull origin main
From https://github.com/some-app/some-repo
 * branch            main       -> FETCH_HEAD
fatal: refusing to merge unrelated histories
C:\dev\app>fix
";
            var lines = consoleBuffer.Split(Environment.NewLine);
            ConsoleHelper.GetCurrentPath = () => @"C:\dev\app";

            var manager = ActionManagerHelper.CreateActionManager();
            var fix = manager.GetFix(lines);

            Assert.Equal(nameof(GitRefusingToMergeUnrelatedHistories), fix.Author);
            Assert.True(fix.IsFixed);
            Assert.Equal("git pull origin main --allow-unrelated-histories", fix.FixedCommand);
        }
    }
}