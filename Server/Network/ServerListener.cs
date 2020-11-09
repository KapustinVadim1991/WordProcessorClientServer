using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace WordProcessorServer.Network
{
    class ServerListener
    {
        /// <summary>
        /// Запускаем сервер и ждем входящих подключений
        /// </summary>
        public static void StartListening(int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);

            try
            {
                listener.Start();

                while (true)
                {
                    if (listener.Pending())
                    {
                        // Каждое новое подключение обрабатываем в новом потоке
                        Task.Run(() =>
                        {
                            var clientObject = new ClientObject(listener.AcceptTcpClient());
                            clientObject.DataExchangeWithClient();
                        });
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
