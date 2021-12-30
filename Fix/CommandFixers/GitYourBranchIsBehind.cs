using System.IO.Enumeration;

namespace Fix.CommandFixers
{
    public class GitYourBranchIsBehind : CommandFixer
    {
        public CommandFix Fix(string lastCommand, string[] consoleBufferInLines)
        {
            if (!lastCommand.StartsWith("git push"))
            {
                return CommandFix.CantFix();
            }

            for (var i = 0; i < consoleBufferInLines.Length; i++)
            {
                if (FileSystemName.MatchesSimpleExpression(" ! [rejected]        * -> * (fetch first)", consoleBufferInLines[i]))
                {
                    return CommandFix.FixesWith(lastCommand.Replace(" push ", " pull "));
                }
            }
            return CommandFix.CantFix();
        }
    }
}