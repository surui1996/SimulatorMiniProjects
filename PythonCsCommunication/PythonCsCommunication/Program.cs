using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Net.Sockets;

namespace PythonCsCommunication
{
    class Program
    {
        static void Main(string[] args)
        {
            string directory = @"C:\try\t.py";
            byte[] directoryBytes = Encoding.Default.GetBytes(directory);
            directory = Encoding.UTF8.GetString(directoryBytes);

            RunPyScript(directory);

            RobotClient client = new RobotClient();
            client.Start();
           
            Console.Read();
        }

        private static void RunPyScript(string scriptName)
        {
            Process p = new Process();
            p.StartInfo.FileName = "python.exe";
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false; // make sure we can read the output from stdout
            p.StartInfo.Arguments = scriptName; // add other parameters if necessary
            p.Start(); // start the process (the python program)
        }
    }
}