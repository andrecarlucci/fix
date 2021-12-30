namespace Fix.CommandFixers
{
    public class ActionManagerFactory
    {
        public ActionManager Create()
        {
            var actionManager = new ActionManager();
            actionManager.AddFix(new GitSimilar());
            actionManager.AddFix(new GitBranchHasNoUpstream());
            actionManager.AddFix(new GitTipOfYourBranchIsBehind());
            actionManager.AddFix(new GitYourBranchIsBehind());
            actionManager.AddFix(new GitRefusingToMergeUnrelatedHistories());
            return actionManager;
        }
    }
}
