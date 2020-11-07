using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    class Program
    {
        private static void Main(string[] args)
        {
            IPAddress ipAddress = GetIPAddress();
            int port = GetPort();

            byte[] bytes = new byte[1024];

            try
            {
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.  
                    byte[] msg = Encoding.UTF8.GetBytes(Console.ReadLine());

                    // Send the data through the socket.  
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.  
                    int bytesRec = sender.Receive(bytes);
                    Console.WriteLine("Echoed test = {0}",
                        Encoding.UTF8.GetString(bytes, 0, bytesRec));

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static IPAddress GetIPAddress()
        {
            while (true)
            {
                Console.Write("Введите ip адрес или hostname сервера: ");
                var input = Console.ReadLine();

                if(IPAddress.TryParse(input, out IPAddress ipAddress))
                {
                    return ipAddress;
                }

                try
                {
                    if (Dns.GetHostEntry(input) != null)
                    {
                        foreach (var ip in Dns.GetHostEntry(input).AddressList)
                        {
                            if (ip.AddressFamily == AddressFamily.InterNetwork)
                            {
                                return ip;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Некорректно указан ip адрес сервера!");
                    }
                }
                catch(SocketException e)
                {
                    Console.WriteLine("Хостнейм не найден");
                }

                            
            }
        }

        private static int GetPort()
        {
            while (true)
            {
                Console.Write("Введите номер порта: ");
                var input = Console.ReadLine();

                if (ushort.TryParse(input, out ushort port))
                {
                    return (int)port;
                }
                else
                {
                    Console.WriteLine("Порт указан неверно!");
                }
            }
        }
    }
}
