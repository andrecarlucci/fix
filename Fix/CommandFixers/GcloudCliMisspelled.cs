using Fix.Helpers;

namespace Fix.CommandFixers
{
    public class GcloudCliMisspelled : ICommandFixer
    {
        public CommandFix Fix(string lastCommand, string[] consoleBufferInLines)
        {
            if (!lastCommand.StartsWith("gcloud "))
            {
                return CommandFix.CantFix();
            }

            for (var i = 0; i < consoleBufferInLines.Length; i++)
            {
                if (consoleBufferInLines[i].StartsWith("ERROR: (gcloud) Invalid choice:") &&
                    consoleBufferInLines[i + 1].StartsWith("Maybe you meant:"))
                {
                    var suggestion = consoleBufferInLines[i + 2].Trim();
                    
                    var command = new Command(lastCommand);
                    var commonStart = command.GetNumberOfLeftCommonWords(suggestion);
                    command.ReplaceWord(suggestion.GetLastWord(), commonStart);
                    return CommandFix.FixesWith(command.Build());
                }
            }
            return CommandFix.CantFix();
        }
    }
}