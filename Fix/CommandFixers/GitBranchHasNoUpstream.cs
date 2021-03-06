using Fix.Helpers;

namespace Fix.CommandFixers
{
    public class GitBranchHasNoUpstream : ICommandFixer
    {
        public CommandFix Fix(string lastCommand, string[] consoleBufferInLines)
        {
            if (!lastCommand.StartsWith("git push"))
            {
                return CommandFix.CantFix();
            }

            for (var i = 0; i < consoleBufferInLines.Length; i++)
            {
                if(consoleBufferInLines[i].MatchesSimpleExpression("fatal: The current branch * has no upstream branch."))
                {
                    var suggested = consoleBufferInLines[i + 2].Trim();
                    return CommandFix.FixesWith(suggested);
                }
            }
            return CommandFix.CantFix();
        }
    }
}