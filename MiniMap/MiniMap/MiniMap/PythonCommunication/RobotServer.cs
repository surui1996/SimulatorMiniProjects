using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using Microsoft.Xna.Framework.Input;
using Simulator.Main;
using Simulator.PhysicalModeling;
using System.Net;
using Simulator.GUI;
using Microsoft.Xna.Framework;

namespace Simulator.PythonCommunication
{
    enum RobotState { Auto, Teleop, Disabled }

    class RobotServer
    {
        Thread serverThread;
        Socket server;
        Robot robot;
        GameBall ball;

        UpdatingList messegesList;

        public RobotServer(Robot robot, GameBall ball, UpdatingList list)
        {
            serverThread = new Thread(new ThreadStart(ListenToClient));
            serverThread.IsBackground = true;

            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            this.robot = robot;
            this.ball = ball;

            this.messegesList = list;
        }

        public void Start()
        {
            serverThread.Start();
        }

        public void Stop()
        {
            server.Send(GetBytes("STOP;"));
            Thread.Sleep(5);
            serverThread.Abort();
        }

        public void SetState(RobotState state)
        {
            server.Send(GetBytes("STATE " + state.ToString().ToUpper()));
        }

        private void ListenToClient()
        {
            server.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4590));
            server.Listen(5);
            server = server.Accept();

            while (true)
            {
                byte[] buffer = new byte[1024];
                server.Receive(buffer);
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
                    else if (requests[i].IndexOf("XKEY") == 0)
                    {
                        string key = requests[i].Split(new string[] { "XKEY " },
                            StringSplitOptions.None)[1];

                        if (SimulatorGame.isXboxKeyPressed(key))
                        {
                            server.Send(GetBytes("XKEY " + key + "=True;"));
                            messegesList.Add("C#: XKEY " + key + "=True;");
                        }
                        else
                            server.Send(GetBytes("XKEY " + key + "=False;"));
                    }
                    else if (requests[i].IndexOf("KEY") == 0)
                    {
                        Keys key = (Keys)Enum.Parse(typeof(Keys),
                            requests[i].Split(new string[] { "KEY " },
                            StringSplitOptions.None)[1]);

                        if (SimulatorGame.keyboardState.IsKeyDown(key))
                        {
                            server.Send(GetBytes("KEY " + key.ToString() + "=True;"));
                            messegesList.Add("C#: KEY " + key.ToString() + "=True;");
                        }
                        else
                            server.Send(GetBytes("KEY " + key.ToString() + "=False;"));
                    }
                    else if (requests[i].IndexOf("ARCADE") == 0)
                    {

                        if (requests[i] == "ARCADE")
                        {
                            robot.ArcadeDrive(GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y, GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.X);
                            messegesList.Add("PY: " + requests[i] + ";");
                        }
                        else
                        {
                            string arcade = requests[i].Split(new string[] { "ARCADE " },
                                StringSplitOptions.None)[1];
                            string[] values = arcade.Split(',');

                            float forward = float.Parse(values[0]); float curve = float.Parse(values[1]);
                            robot.ArcadeDrive(forward, curve);
                            messegesList.Add("PY: ARCADE " + forward.ToString("0.00") + "," + curve.ToString("0.00") + ";");
                        }

                        server.Send(GetBytes("LeftOutput" + "=" + robot.LeftOutput + ";"));
                        server.Send(GetBytes("RightOutput" + "=" + robot.RightOutput + ";"));
                    }
                    else if (requests[i].IndexOf("TANK") == 0)
                    {
                        if (requests[i] == "TANK")
                        {
                            robot.TankDrive(GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y, GamePad.GetState(PlayerIndex.One).ThumbSticks.Right.Y);
                            server.Send(GetBytes("LeftOutput" + "=" + robot.LeftOutput + ";"));
                            server.Send(GetBytes("RightOutput" + "=" + robot.RightOutput + ";"));
                            messegesList.Add("PY: " + requests[i]);
                        }
                        else
                        {
                            string tank = requests[i].Split(new string[] { "TANK " },
                            StringSplitOptions.None)[1];
                            string[] values = tank.Split(',');

                            float left = float.Parse(values[0]); float right = float.Parse(values[1]);
                            robot.TankDrive(left, right);
                            messegesList.Add("PY: TANK " + left.ToString("0.00") + "," + right.ToString("0.00") + ";");
                        }
                    }
                    else if (requests[i].IndexOf("RESET") == 0)
                    {
                        messegesList.Add("PY: " + requests[i] + ";");
                        if (requests[i].IndexOf("ENCODERS") != -1)
                            robot.ResetEncoders();
                        else if (requests[i].IndexOf("GYRO") != -1)
                            robot.ResetGyro();
                    }
                    else if (requests[i].IndexOf("POSSESS") == 0)
                    {
                        messegesList.Add("PY: " + requests[i] + ";");
                        if (Math.Abs((robot.Position - ball.Position).Length()) < 1)
                        {
                            ball.PutOnRobot();
                            server.Send(GetBytes("POSSESS True;"));
                            messegesList.Add("C#: POSSESS True;");
                        }
                        else
                            server.Send(GetBytes("POSSESS False;"));
                    }
                    else if (requests[i].IndexOf("SHOOT") == 0)
                    {
                        messegesList.Add("PY: " + requests[i] + ";");
                        ball.ShootBall(robot.Orientation, robot.Velocity);
                    }

                }
            }
        }

        private void ParseGetRequest(string get)
        {
            switch (get)
            {
                case "EncoderLeft":
                    server.Send(GetBytes(get + "=" + robot.EncoderLeft + ";"));
                    break;
                case "EncoderRight":
                    server.Send(GetBytes(get + "=" + robot.EncoderRight + ";"));
                    break;
                case "Gyro":
                    server.Send(GetBytes(get + "=" + robot.GyroAngle + ";"));
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
