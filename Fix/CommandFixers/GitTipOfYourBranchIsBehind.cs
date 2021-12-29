using System;
using System.IO.Enumeration;
using System.Text.RegularExpressions;

namespace Fix.CommandFixers
{
    public class GitTipOfYourBranchIsBehind : CommandFixer
    {
        public CommandFix Fix(string lastCommand, string[] consoleBufferInLines)
        {
            if (!lastCommand.StartsWith("git push --set-upstream"))
            {
                return CommandFix.CantFix();
            }

            for (var i = 0; i < consoleBufferInLines.Length; i++)
            {
                if (FileSystemName.MatchesSimpleExpression(" ! [rejected]        * -> * (non-fast-forward)", consoleBufferInLines[i]))
                {
                    var branch = consoleBufferInLines[i].Split(" ")[10];
                    return CommandFix.FixesWith("git pull origin " + branch);
                }
            }
            return CommandFix.CantFix();
        }
    }
}