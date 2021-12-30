using Fix.CommandFixers;

namespace Fix.Tests
{
    public static class ActionManagerHelper
    {
        public static ActionManager CreateActionManager()
        {
            return new ActionManagerFactory().Create();
        }
    }
}
