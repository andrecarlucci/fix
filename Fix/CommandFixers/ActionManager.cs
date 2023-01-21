using Fix.ConsoleHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Fix.CommandFixers
{
    public class ActionManager
    {
        private static readonly Regex _normalizerRegex = new("^(PS )(.*>)(\\s)?(.*)", RegexOptions.Compiled);
        private List<ICommandFixer> _fixActions = new();

        public string LastCommand { get; private set; } = "";

        public void AddFix(ICommandFixer fixAction)
        {
            _fixActions.Add(fixAction);
        }

        private string GetLastCommand(IEnumerable<string> lines)
        {
            var currentPath = ConsoleHelper.GetCurrentPath();
            var commandPrefix = currentPath + ConsoleHelper.CONSOLE_SEPARATOR;
            var lastCommandLine = lines.Where(x => x.StartsWith(commandPrefix))
                                       .Reverse()
                                       .Skip(1)
                                       .FirstOrDefault();
            if (lastCommandLine == null)
            {
                return "";
            }
            var index = lastCommandLine.IndexOf(ConsoleHelper.CONSOLE_SEPARATOR);
            return lastCommandLine[(index + 1)..].Trim();
        }

        private string[] FilterLastCommand(string lastCommand, IEnumerable<string> lines)
        {
            var currentPath = ConsoleHelper.GetCurrentPath();
            var fullLastCommand = $"{currentPath}{ConsoleHelper.CONSOLE_SEPARATOR}{lastCommand}";
            return lines.Reverse()
                        .Skip(1)
                        .TakeWhile(x => x != fullLastCommand)
                        .Reverse()
                        .ToArray();
        }

        public CommandFix GetFix(string[] consoleBufferInLines)
        {
            var normalizedBuffer = consoleBufferInLines.Select(s => _normalizerRegex.Replace(s, "$2$4"));
            LastCommand = GetLastCommand(normalizedBuffer);
            var lastCommandResult = FilterLastCommand(LastCommand, normalizedBuffer);

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
