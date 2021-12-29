using System.IO;
using System.Runtime.CompilerServices;

namespace Fix.CommandFixers
{
    public class CommandFix
    {
        public CommandFix(bool isFixed, string fixedCommand, string author)
        {
            IsFixed = isFixed;
            FixedCommand = fixedCommand;
            Author = author;
        }

        public string Author { get; }
        public bool IsFixed { get; }
        public string FixedCommand { get; }

        public static CommandFix CantFix([CallerFilePath] string callerFilePath = null)
        {
            var callerTypeName = Path.GetFileNameWithoutExtension(callerFilePath);
            return new CommandFix(false, "", callerTypeName);
        }

        public static CommandFix FixesWith(string newCommand, [CallerFilePath] string callerFilePath = null)
        {
            var callerTypeName = Path.GetFileNameWithoutExtension(callerFilePath);
            return new CommandFix(true, newCommand, callerTypeName);
        }
    }
}
