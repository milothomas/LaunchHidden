using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LaunchHidden
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            if (!TryParseArgs(args, out var cmdPath, out var cmdArgs))
            {
                ShowHelp();
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = Directory.GetCurrentDirectory(),
                Arguments = cmdArgs,
                FileName = cmdPath
            };

            try
            {
                var proc = Process.Start(startInfo);
                if (proc == null)
                {
                    Console.WriteLine("Error starting process");
                    return;
                }

                proc.StandardInput.Close();
                proc.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting process. {ex.Message}");
            }
        }

        private static bool TryParseArgs(string[] args, out string cmdPath, out string cmdArgs)
        {
            cmdPath = null;
            cmdArgs = null;

            if (args == null || !args.Any())
            {
                return false;
            }

            cmdPath = args.FirstOrDefault()?.Trim();
            cmdArgs = args.Skip(1).FirstOrDefault()?.Trim();

            return cmdPath != null;
        }

        private static void ShowHelp()
        {
            var module = Process.GetCurrentProcess().MainModule;
            var filePath = module != null
                ? module.FileName
                : Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            
            var fi = new FileInfo(filePath);
            var fileName = fi.Name;

            Console.WriteLine("Invalid arguments");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Usage");
            Console.WriteLine("=================");
            Console.WriteLine($"  {fileName} <PATH TO EXECUTABLE> <ARGUMENTS>");
            Console.WriteLine($"  {fileName} notepad.exe \"this is a test\"");
            Console.WriteLine("=================");
            Console.WriteLine("");


        }
    }
}