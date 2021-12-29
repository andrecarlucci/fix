using Fix.ConsoleHelpers;
using Fix.CommandFixers;
using System;

namespace Fix
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length > 0 && args[0].ToLower() == "debug")
            {
                ConsoleHelper.Debug = true;
            }

            var lines = ConsoleHelper.ReadActiveBufferAsLines();

            if (lines.Length < 3)
            {
                Console.WriteLine("Nothing to fix");
            }
            
            var actionManager = new ActionManager();
            actionManager.AddFix(new GitSimilar());
            var command = actionManager.GetFix(lines);

            if(!command.IsFixed)
            {

                Console.WriteLine("Sorry, I don't know how to fix: " + actionManager.LastCommand);
                return;
            }
            try
            {
                ConsoleHelper.Log("Commmand to be executed: " + command.FixedCommand);
                ConsoleHelper.ExecuteCmd(command.FixedCommand);
            }
            catch(Exception e)
            {
                ConsoleHelper.Log("Error executing command: " + e);
            }
        }
    }
}
