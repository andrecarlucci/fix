using System.IO.Enumeration;

namespace Fix.CommandFixers
{
    public class GitBranchHasNoUpstream : CommandFixer
    {
        public CommandFix Fix(string lastCommand, string[] consoleBufferInLines)
        {
            if (!lastCommand.StartsWith("git push"))
            {
                return CommandFix.CantFix();
            }

            for (var i = 0; i < consoleBufferInLines.Length; i++)
            {
                if(FileSystemName.MatchesSimpleExpression("fatal: The current branch * has no upstream branch.", consoleBufferInLines[i]))
                {
                    var suggested = consoleBufferInLines[i + 3].Trim();
                    return CommandFix.FixesWith(suggested);
                }
            }
            return CommandFix.CantFix();
        }
    }
}