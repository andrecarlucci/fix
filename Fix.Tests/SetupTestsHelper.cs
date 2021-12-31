using Fix.CommandFixers;
using Fix.ConsoleHelpers;

namespace Fix.Tests
{
    public static class SetupTestsHelper
    {
        public const string FakePath = @"C:\dev\app";

        public static ActionManager CreateActionManager()
        {
            ConsoleHelper.Debug = true;
            ConsoleHelper.GetCurrentPath = () => FakePath;
            return new ActionManagerFactory().Create();
        }
    }
}
