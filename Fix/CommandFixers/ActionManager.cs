using Fix.ConsoleHelpers;
using System.Collections.Generic;
using System.Linq;

namespace Fix.CommandFixers
{
    public class ActionManager
    {
        private List<ICommandFixer> _fixActions = new();

        public string LastCommand { get; private set; } = "";

        public void AddFix(ICommandFixer fixAction)
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

        private string[] FilterLastCommand(string lastCommand, string[] lines)
        {
            var currentPath = ConsoleHelper.GetCurrentPath();
            return lines.Reverse()
                        .Skip(1)
                        .TakeWhile(x => x != $"{currentPath}{ConsoleHelper.CONSOLE_SEPARATOR}{lastCommand}")
                        .Reverse()
                        .ToArray();
        }

        public CommandFix GetFix(string[] consoleBufferInLines)
        {
            LastCommand = GetLastCommand(consoleBufferInLines);
            var lastCommandResult = FilterLastCommand(LastCommand, consoleBufferInLines);

            LogBuffer(lastCommandResult);

            foreach (var fix in _fixActions)
            {
                var fixResult = fix.Fix(LastCommand, lastCommandResult);
                if (fixResult.IsFixed)
                {
                    ConsoleHelper.Log($"Fixer {fix.GetType().Name} FIXED IT!");
                    return fixResult;
                }
                else
                {
                    ConsoleHelper.Log($"Fixer {fix.GetType().Name} cannot fix it");
                }
            }
            return new CommandFix(false, "", nameof(ActionManager));
        }

        private void LogBuffer(string[] consoleBufferInLines)
        {
            ConsoleHelper.Log("LastCommand:" + LastCommand);

            ConsoleHelper.Log("---------------");
            ConsoleHelper.Log("Buffer:");
            ConsoleHelper.Log("---------------");
            for (var i = 0; i < consoleBufferInLines.Length; i++)
            {
                ConsoleHelper.Log(consoleBufferInLines[i]);
            }
            ConsoleHelper.Log("---------------");
        }
    }
}
