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

            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            client.Connect("127.0.0.1", 4590);
            float value = 0.0f;
            while (true)
            {
                value += 0.001f;
                client.Send(GetBytes("Gyro=" + value.ToString() + ";"));
                byte[] buffer = new byte[1024];
                client.Receive(buffer);
                Console.WriteLine(GetString(buffer));
                System.Threading.Thread.Sleep(20);
            }
        }

        private static string GetString(byte[] text)
        {
            return System.Text.Encoding.ASCII.GetString(text).Split('\0')[0];
        }

        private static byte[] GetBytes(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

        private static void RunPyScript(string scriptName)
        {
            Process p = new Process();
            p.StartInfo.FileName = "python.exe";
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false; // make sure we can read the output from stdout
            p.StartInfo.Arguments = scriptName; // add other parameters if necessary
            p.Start(); // start the process (the python program)
            
            //get the output
            /*StreamReader s = p.StandardOutput;
            String output = s.ReadToEnd();
            Console.WriteLine(output);
            p.WaitForExit();*/
        }
    }
}

