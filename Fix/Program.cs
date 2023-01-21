using Fix.ConsoleHelpers;
using Fix.CommandFixers;
using System;
using System.Linq;

namespace Fix
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Contains("-debug"))
            {
                ConsoleHelper.Debug = true;
            }

            if (args.Contains("-plan"))
            {
                ConsoleHelper.Plan = true;
            }

            var lines = ConsoleHelper.ReadActiveBufferAsLines();

            if (lines.Length < 3)
            {
                Console.WriteLine("Nothing to fix");
                return;
            }
            
            var actionManager = new ActionManagerFactory().Create();
            
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
