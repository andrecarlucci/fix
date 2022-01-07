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
                if (consoleBufferInLines[i].Contains(" is misspelled or not recognized by the system."))
                {
                    var wrongWord = StringFinder.FindFirstWordBetweenSingleQuotes(consoleBufferInLines[i]);
                    var correctWord = StringFinder.FindFirstWordBetweenSingleQuotes(consoleBufferInLines[i+1]);
                    var newCommand = StringFinder.ReplaceFirst(lastCommand, wrongWord, correctWord);
                    return CommandFix.FixesWith(newCommand);
                }
            }
            return CommandFix.CantFix();
        }
    }
}