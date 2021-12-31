using System.Text.RegularExpressions;

namespace Fix.CommandFixers
{
    public class DosSimpleTypo : ICommandFixer
    {
        public CommandFix Fix(string lastCommand, string[] consoleBufferInLines)
        {
            if (!lastCommand.StartsWith("dri"))
            {
                return CommandFix.CantFix();
            }

            for (var i = 0; i < consoleBufferInLines.Length; i++)
            {
                if (consoleBufferInLines[i] == "'dri' is not recognized as an internal or external command,")
                {
                    var regex = new Regex(Regex.Escape("dri"));
                    var newCommand = regex.Replace(lastCommand, "dir", 1);
                    return CommandFix.FixesWith(newCommand);
                }
            }
            return CommandFix.CantFix();
        }
    }
}