using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Net;
using System.Net.Sockets;
namespace RobotSimulator
{
    class MJPEGStreamer
    {
        //private const int FPS = 30;

        Thread streamThread;
        public byte[] Frame;

        private bool hasFrameRecieved;
        public bool FrameRecieved
        {
            get
            {
                bool b = hasFrameRecieved;
                hasFrameRecieved = false;
                return b;
            }
            set
            {
                hasFrameRecieved = value;
            }
        }

        public MJPEGStreamer()
        {
            streamThread = new Thread(new ThreadStart(HandleStream));
           
            FrameRecieved = false;
            Frame = null;
        }

        public void Start()
        {
            streamThread.Start();
        }

        public void OnNewFrame(byte[] imgBuffer)
        {
            byte[] header = CreateHeader(imgBuffer.Length);
            byte[] footer = CreateFooter();
            
            byte[] frame = new byte[header.Length + imgBuffer.Length + footer.Length];
            System.Array.Copy(header, 0, frame, 0, header.Length);
            System.Array.Copy(imgBuffer, 0, frame, header.Length, imgBuffer.Length);
            System.Array.Copy(footer, 0, frame, header.Length + imgBuffer.Length, footer.Length);
            Frame = frame;
            FrameRecieved = true;
        }

        private byte[] CreateHeader(int length)
        {
            // what is this boundry????
            string header = "--boundry\r\nContent-Type:image/jpeg\r\nContent-Length:" + length + "\r\n\r\n";

            // using ascii encoder is fine since there is no international character used in this string.
            return ASCIIEncoding.ASCII.GetBytes(header);
        }

        public byte[] CreateFooter()
        {
            return ASCIIEncoding.ASCII.GetBytes("\r\n");
        }
        Stream serverStream;
        public void HandleStream()
        {
            Socket Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            Server.Bind(new IPEndPoint(IPAddress.Any, 4590));
            Server.Listen(10);
            Socket socketForClient = Server.Accept();
            serverStream = new NetworkStream(socketForClient);
            //System.Diagnostics.Debug.WriteLine(string.Format("Server started on port {0}.", port));
            Write(
               "HTTP/1.1 200 OK\r\n" +
               "Content-Type: multipart/x-mixed-replace; boundary=" +
                "--boundary" +
               "\r\n"
            );

            this.serverStream.Flush();
           

            //IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            //TcpListener tcpListener = new TcpListener(localAddr, 65000);
            //tcpListener.Start();
            //Socket socketForClient = tcpListener.AcceptSocket();
            
            //NetworkStream networkStream = new NetworkStream(socketForClient);
            //socketForClient.Close();
            while (true)
            {
                if (Frame != null)
                {
                    serverStream.Write(Frame, 0, Frame.Length);
                    Frame = null;
                }
            }
        }
        private void Write(string text)
        {
            byte[] data = BytesOf(text);
            this.serverStream.Write(data, 0, data.Length);
        }

        private static byte[] BytesOf(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

    }
}