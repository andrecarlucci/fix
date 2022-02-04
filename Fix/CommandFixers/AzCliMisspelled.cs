using Fix.Helpers;

namespace Fix.CommandFixers
{
    public class AzCliMisspelled : ICommandFixer
    {
        public CommandFix Fix(string lastCommand, string[] consoleBufferInLines)
        {
            if (!lastCommand.StartsWith("az "))
            {
                return CommandFix.CantFix();
            }

            for (var i = 0; i < consoleBufferInLines.Length; i++)
            {
                if (consoleBufferInLines[i].Contains(" is misspelled or not recognized by the system.") &&
                    consoleBufferInLines[i + 1].StartsWith("Did you mean"))
                {
                    var wrongWord = consoleBufferInLines[i].FindFirstWordBetweenSingleQuotes();
                    var correctWord = consoleBufferInLines[i + 1].FindFirstWordBetweenSingleQuotes();
                    var newCommand = lastCommand.ReplaceFirst(wrongWord, correctWord);
                    return CommandFix.FixesWith(newCommand);
                }
            }
            return CommandFix.CantFix();
        }
    }
}
