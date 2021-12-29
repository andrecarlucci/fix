using Fix.ConsoleHelpers;
using System.Collections.Generic;
using System.Linq;

namespace Fix.CommandFixers
{
    public class ActionManager
    {
        private List<CommandFixer> _fixActions = new();

        public string LastCommand { get; private set; } = "";

        public void AddFix(CommandFixer fixAction)
        {
            _fixActions.Add(fixAction);
        }

        private string GetLastCommand(string[] lines)
        {
            var currentPath = ConsoleHelper.GetCurrentPath();
            var lastCommandLine = lines.Where(x => x.StartsWith(currentPath + ConsoleHelper.CONSOLE_SEPARATOR))
                                       .Reverse()
                                       .Skip(1)
                                       .FirstOrDefault();
            if (lastCommandLine == null)
            {
                return "";
            }
            var index = lastCommandLine.IndexOf(ConsoleHelper.CONSOLE_SEPARATOR);
            return lastCommandLine[(index + 1)..];
        }

        public CommandFix GetFix(string[] consoleBufferInLines)
        {
            LastCommand = GetLastCommand(consoleBufferInLines);
            ConsoleHelper.Log("LastCommand:" + LastCommand);

            foreach (var fix in _fixActions)
            {
                var fixResult = fix.Fix(LastCommand, consoleBufferInLines);
                if(fixResult.IsFixed)
                {
                    ConsoleHelper.Log($"Fixer {fix.GetType().Name} FIXED!");
                    return fixResult;
                }
                else
                {
                    ConsoleHelper.Log($"Fixer {fix.GetType().Name} cannot fix it");
                }
            }
            return new CommandFix(false, "", nameof(ActionManager));
        }
    }
}
