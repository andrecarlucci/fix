namespace Fix.CommandFixers
{
    public interface ICommandFixer
    {
        CommandFix Fix(string lastCommand, string[] consoleBufferInLines);
    }
}