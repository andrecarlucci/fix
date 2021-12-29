using Fix.CommandFixers;

namespace Fix.Tests
{
    public static class ActionManagerHelper
    {
        public static ActionManager CreateActionManager()
        {
            var actionManager = new ActionManager();
            actionManager.AddFix(new GitSimilar());
            actionManager.AddFix(new GitBranchHasNoUpstream());
            actionManager.AddFix(new GitTipOfYourBranchIsBehind());
            actionManager.AddFix(new GitRefusingToMergeUnrelatedHistories());
            
            return actionManager;
        }
    }
}
