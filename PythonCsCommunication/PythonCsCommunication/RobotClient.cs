using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace PythonCsCommunication
{
    class RobotClient
    {
        Thread clientThread;
        Socket client;
        Dictionary<string, float> robotValues;

        public RobotClient()
        {
            clientThread = new Thread(new ThreadStart(ListenToServer));
            clientThread.IsBackground = true;

            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            robotValues = new Dictionary<string, float>();
            robotValues.Add("LeftOutput", 0.0f);
            robotValues.Add("RightOutput", 0.0f);
            robotValues.Add("EncoderLeft", 0.0f);
            robotValues.Add("EncoderRight", 0.0f);
            robotValues.Add("Gyro", 0.0f);
            robotValues.Add("Accelerometer", 0.0f);
        }

        public void Start()
        {
            clientThread.Start();
        }

        private void ListenToServer()
        {
            client.Connect("127.0.0.1", 4590);
            
            while (true)
            {
                byte[] buffer = new byte[1024];
                client.Receive(buffer);
                string request = GetString(buffer);

                try
                {
                    ParseRequests(request);
                }
                catch { }
                

                Console.WriteLine(request);
                robotValues["Gyro"] += 0.01f;
            }
        }

        private void ParseRequests(string request)
        {
            string[] requests = request.Split(';');
            for (int i = 0; i < requests.Length; i++)
            {
                if (requests[i].Length != 0)
                {
                    string getBody = requests[i].Split(' ')[1];
                    if (requests[i].IndexOf("GET") == 0)
                    {
                        ParseGetRequest(getBody);
                    }
                    else if (requests[i].IndexOf("SET") == 0)
                    {
                        string[] set = getBody.Split('=');
                        ParseSetRequest(set[0], float.Parse(set[1]));
                    }
                }
            }
        }

        private void ParseGetRequest(string get)
        {
            client.Send(GetBytes(get + "=" + robotValues[get] + ";"));
        }

        private void ParseSetRequest(string set, float value)
        {
            robotValues[set] = value;
        }

        private static string GetString(byte[] text)
        {
            return System.Text.Encoding.ASCII.GetString(text).Split('\0')[0];
        }

        private static byte[] GetBytes(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }
    }
}
