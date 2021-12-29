using System.Collections.Generic;

namespace Fix.CommandFixers
{
    public interface CommandFixer
    {
        CommandFix Fix(string lastCommand, string[] consoleBufferInLines);
    }
}