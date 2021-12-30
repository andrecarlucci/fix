namespace Fix.CommandFixers
{
    public class GitRefusingToMergeUnrelatedHistories : CommandFixer
    {
        public CommandFix Fix(string lastCommand, string[] lines)
        {
            if (!lastCommand.StartsWith("git pull "))
            {
                return CommandFix.CantFix();
            }

            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i] == "fatal: refusing to merge unrelated histories")
                {
                    return CommandFix.FixesWith(lastCommand + " --allow-unrelated-histories");
                }
            }
            return CommandFix.CantFix();
        }
    }
}