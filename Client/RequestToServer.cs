using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    static class RequestToServer
    {
        /// <summary>
        /// Отправляет на сервер введенный пользователем префикс и получает ответ от сервера.
        /// </summary>
        /// <returns>Возвращает список слов, либо выводит в консоль сообщение об ошибке</returns>
        public static List<string> GetTopFiveWords(IPAddress ipAddress, int port, string prefix)
        {
            byte[] bytes = new byte[1024];

            try
            {
                IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
                Socket sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    sender.Connect(remoteEP);
                    byte[] msg = Encoding.UTF8.GetBytes(prefix);
                    sender.Send(msg);
                    int bytesRec = sender.Receive(bytes);
                    var recieve = Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    sender.Shutdown(SocketShutdown.Both);

                    if (recieve.StartsWith("Error:"))
                    {
                        Console.WriteLine(recieve);
                        return new List<string>();
                    }
                    return recieve.Split(' ').ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    sender.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return new List<string>();
        }

    }
}
