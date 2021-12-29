using Mischel.ConsoleDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Fix.ConsoleHelpers
{
    public static class ConsoleHelper
    {
        public static bool Debug = false;
        public static bool Plan = false;


        public const string CONSOLE_SEPARATOR = ">";
        public static Func<string> GetCurrentPath { get; set; } = () => Environment.CurrentDirectory;

        public static void ExecuteCmd(string command)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/c " + command,
                RedirectStandardInput = false,
                UseShellExecute = false
            };
            Console.WriteLine(GetCurrentPath() + CONSOLE_SEPARATOR + command);
            if(Plan)
            {
                return;
            }
            Process.Start(startInfo).WaitForExit();
        }

        public static string[] ReadActiveBufferAsLines()
        {
            var lines = new List<string>();
            using ConsoleScreenBuffer sb = JConsole.GetActiveScreenBuffer();
            var h = sb.Height;
            var w = sb.Width;

            var buffer = new StringBuilder();
            for (var i = 0; i < h; i++)
            {
                var line = sb.ReadXY(w, 0, i).TrimEnd();
                if (!String.IsNullOrWhiteSpace(line))
                {
                    lines.Add(line);
                }
            }
            return lines.ToArray();
        }


        public static void Log(string line)
        {
            if(!Debug)
            {
                return;
            }
            Console.WriteLine("Fix -> " + line);
        }
    }
}