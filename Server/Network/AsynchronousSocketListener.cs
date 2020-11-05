using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WordProcessor.Network
{
    class AsynchronousSocketListener
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public static void StartListening(int portNumber)
        {
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAdress = ipHost.AddressList[1];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAdress, portNumber);

            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(ipEndPoint);
                listener.Listen(100);

                while (true)
                {
                    allDone.Reset();
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            allDone.Set();

            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            StateObject stateObject = new StateObject(handler);
            handler.BeginReceive(stateObject.buffer, 0, StateObject.BufferSize, SocketFlags.None, 
                new AsyncCallback(ReadCallback), stateObject);
        }

        private static void ReadCallback(IAsyncResult ar)
        {
            var content = String.Empty;

            var stateObject = (StateObject)ar.AsyncState;
            Socket handler = stateObject.clientSocket;

            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                stateObject.recievedData.Append(Encoding.UTF8.GetString(stateObject.buffer, 0, bytesRead));
                handler.BeginReceive(stateObject.buffer, 0, StateObject.BufferSize, SocketFlags.None,
                        new AsyncCallback(ReadCallback), stateObject);                
            }
            else
            {
                content = stateObject.recievedData.ToString();
                if (content.StartsWith("get "))
                {
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);
                    string topFive = RequestsToDatabase.FindTopFive(content.Substring(4).Trim());

                    Send(handler, topFive);
                }
            }
        }

        private static void Send(Socket handler, string data)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            handler.BeginSend(byteData, 0, byteData.Length, SocketFlags.None,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                var handler = (Socket)ar.AsyncState;
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);
                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
