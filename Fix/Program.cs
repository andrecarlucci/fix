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
            if(args.Any(x => x == "-debug"))
            {
                ConsoleHelper.Debug = true;
            }
            if (args.Any(x => x == "-plan"))
            {
                ConsoleHelper.Plan = true;
            }

            var lines = ConsoleHelper.ReadActiveBufferAsLines();

            if (lines.Length < 3)
            {
                Console.WriteLine("Nothing to fix");
            }
            
            var actionManager = new ActionManager();
            actionManager.AddFix(new GitSimilar());
            actionManager.AddFix(new GitBranchHasNoUpstream());
            actionManager.AddFix(new GitTipOfYourBranchIsBehind());
            actionManager.AddFix(new GitRefusingToMergeUnrelatedHistories());
            
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
