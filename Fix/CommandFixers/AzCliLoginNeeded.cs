using Fix.Helpers;

namespace Fix.CommandFixers
{
    public class AzCliLoginNeeded : ICommandFixer
    {
        public CommandFix Fix(string lastCommand, string[] consoleBufferInLines)
        {
            if (!lastCommand.StartsWith("az "))
            {
                return CommandFix.CantFix();
            }

            for (var i = 0; i < consoleBufferInLines.Length; i++)
            {
                if (consoleBufferInLines[i].MatchesSimpleExpression("User * does not exist in MSAL token cache. Run `az login`."))
                {
                    return CommandFix.FixesWith("az login");
                }
            }
            return CommandFix.CantFix();
        }
    }
}