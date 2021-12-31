using System;
using System.Linq;

namespace Fix.CommandFixers
{
    public class ActionManagerFactory
    {
        public ActionManager Create()
        {
            var type = typeof(ICommandFixer);
            var types = type.Assembly
                            .GetTypes()
                            .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract)
                            .ToList();

            var actionManager = new ActionManager();
            types.ForEach(t => actionManager.AddFix((ICommandFixer)Activator.CreateInstance(t)));
            //actionManager.AddFix(new GitSimilar());
            //actionManager.AddFix(new GitBranchHasNoUpstream());
            //actionManager.AddFix(new GitTipOfYourBranchIsBehind());
            //actionManager.AddFix(new GitYourBranchIsBehind());
            //actionManager.AddFix(new GitRefusingToMergeUnrelatedHistories());
            //actionManager.AddFix(new DosSimpleTypo());
            return actionManager;
        }
    }
}
