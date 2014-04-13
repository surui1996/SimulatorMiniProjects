using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using Microsoft.Xna.Framework.Input;

namespace MiniMap
{
    class RobotClient
    {
        Thread clientThread;
        Socket client;
        Robot robot;      

        public RobotClient(Robot robot)
        {
            clientThread = new Thread(new ThreadStart(ListenToServer));
            clientThread.IsBackground = true;

            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            this.robot = robot;
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

                Thread.Sleep(5);
            }
        }

        private void ParseRequests(string request)
        {
            string[] requests = request.Split(';');
            for (int i = 0; i < requests.Length; i++)
            {
                if (requests[i].Length != 0)
                {
                    if (requests[i].IndexOf("GET") == 0)
                        ParseGetRequest(requests[i].Split(' ')[1]);
                    else if (requests[i].IndexOf("KEY") == 0)
                    {
                        Keys key = (Keys)Enum.Parse(typeof(Keys),
                            requests[i].Split(new string[] { "KEY " },
                            StringSplitOptions.None)[1]);

                        //TODO: find a prettier way..
                        if(Game1.keyboardState.IsKeyDown(Keys.Space))
                            client.Send(GetBytes("KEY " + key.ToString() +  "=True;"));
                        else
                            client.Send(GetBytes("KEY " + key.ToString() + "=False;")); 
                    }
                    else if (requests[i].IndexOf("ARCADE") == 0)
                    {
                        string arcade = requests[i].Split(new string[] { "ARCADE " },
                            StringSplitOptions.None)[1];
                        string[] values = arcade.Split(',');
                        robot.ArcadeDrive(float.Parse(values[0]), float.Parse(values[1]));
                        client.Send(GetBytes("LeftOutput" + "=" + robot.LeftOutput + ";"));
                        client.Send(GetBytes("RightOutput" + "=" + robot.RightOutput + ";"));
                    }
                    else if (requests[i].IndexOf("TANK") == 0)
                    {
                        string tank = requests[i].Split(new string[] { "TANK " },
                            StringSplitOptions.None)[1];
                        string[] values = tank.Split(',');
                        robot.TankDrive(float.Parse(values[0]), float.Parse(values[1]));
                    }
                    else if (requests[i].IndexOf("RESET") == 0)
                    {
                        if (requests[i].IndexOf("ENCODERS") != -1)
                            robot.ResetEncoders();
                        else if (requests[i].IndexOf("GYRO") != -1)
                            robot.ResetGyro();
                    }

                    //TODO: implemet "PRESSED" message
                }
            }
        }

        private void ParseGetRequest(string get)
        {
            switch (get)
            {
                case "EncoderLeft":
                    client.Send(GetBytes(get + "=" + robot.EncoderLeft + ";"));
                    break;
                case "EncoderRight":
                    client.Send(GetBytes(get + "=" + robot.EncoderRight + ";"));
                    break;
                case "Gyro":
                    client.Send(GetBytes(get + "=" + robot.GyroAngle + ";"));
                    break;
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

    }
}
