namespace Fix.CommandFixers
{
    public class GitSimilar : ICommandFixer
    {
        public CommandFix Fix(string lastCommand, string[] consoleBufferInLines)
        {
            if (!lastCommand.StartsWith("git "))
            {
                return CommandFix.CantFix();
            }

            for (var i = 0; i < consoleBufferInLines.Length; i++)
            {
                if (consoleBufferInLines[i].StartsWith("The most similar "))
                {
                    var first = consoleBufferInLines[i + 1].Trim();
                    return CommandFix.FixesWith("git " + first);
                }
            }
            return CommandFix.CantFix();
        }
    }
}