using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Drawing;
using System.IO;
using System.Net;

namespace RobotSimulator
{
    /// <summary>
    /// Provides a streaming server that can be used to stream any images source
    /// to any client.
    /// </summary>
    public class ImageStreamingServer : IDisposable
    {
        private List<Socket> streamClients;
        private Thread serverThread;

        public ImageStreamingServer()
        {
            streamClients = new List<Socket>();
            serverThread = null;

            this.Interval = 50;
        }

        /// <summary>
        /// Gets or sets the interval in milliseconds (or the delay time) between 
        /// the each image and the other of the stream (the default is . 
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// Gets a collection of client sockets.
        /// </summary>
        public IEnumerable<Socket> Clients { get { return streamClients; } }

        /// <summary>
        /// Returns the status of the server. True means the server is currently 
        /// running and ready to serve any client requests.
        /// </summary>
        public bool IsRunning { get { return (serverThread != null && serverThread.IsAlive); } }

        /// <summary>
        /// Starts the server to accepts any new connections on the specified port.
        /// </summary>
        /// <param name="port"></param>
        public void Start(int port)
        {
            lock (this)
            {
                serverThread = new Thread(new ParameterizedThreadStart(ServerThread));
                serverThread.IsBackground = true;
                serverThread.Start(port);
            }

        }

        /// <summary>
        /// Starts the server to accepts any new connections on the default port (8080).
        /// </summary>
        public void Start()
        {
            this.Start(8080);
        }

        public void Stop()
        {

            if (this.IsRunning)
            {
                try
                {
                    serverThread.Join();
                    serverThread.Abort();
                }
                finally
                {

                    lock (streamClients)
                    {

                        foreach (var s in streamClients)
                        {
                            try
                            {
                                s.Close();
                            }
                            catch { }
                        }
                        streamClients.Clear();

                    }

                    serverThread = null;
                }
            }
        }

        /// <summary>
        /// This the main thread of the server that serves all the new 
        /// connections from clients.
        /// </summary>
        /// <param name="port"></param>
        private void ServerThread(object port)
        {
            try
            {
                Socket Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                Server.Bind(new IPEndPoint(IPAddress.Any, (int)port));
                Server.Listen(10);

                System.Diagnostics.Debug.WriteLine(string.Format("Server started on port {0}.", port));

                foreach (Socket client in Server.IncommingConnections())
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), client);

            }
            catch { }

            this.Stop();
        }

        /// <summary>
        /// Each client connection will be served by this thread.
        /// </summary>
        /// <param name="client"></param>
        private void ClientThread(object client)
        {
            Socket socket = (Socket)client;

            //System.Diagnostics.Debug.WriteLine(string.Format("New client from {0}", socket.RemoteEndPoint.ToString()));

            lock (streamClients)
                streamClients.Add(socket);

            try
            {
                using (MjpegWriter wr = new MjpegWriter(new NetworkStream(socket, true)))
                {

                    // Writes the response header to the client.
                    wr.WriteHeader();

                    while (true)
                    {
                        if (Game1.FrameRecieved)
                        {
                            if (Game1.ImageBuffer != null)
                            {
                                lock (Game1.locker)
                                {
                                    wr.Write(Game1.ImageBuffer);
                                    Game1.ImageBuffer = null;
                                }
                                
                                //Game1.ImageStream.Dispose();
                            }
                            Game1.FrameRecieved = false;
                        }

                        if (this.Interval > 0)
                            Thread.Sleep(this.Interval);
                    }

                }
            }
            catch { }
            finally
            {
                lock (streamClients)
                    streamClients.Remove(socket);
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            this.Stop();
        }

        #endregion
    }

    static class SocketExtensions
    {
        public static IEnumerable<Socket> IncommingConnections(this Socket server)
        {
            while (true)
                yield return server.Accept();
        }
    }
}

