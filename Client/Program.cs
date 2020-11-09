using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace Client
{
    class Program
    {
        private static void Main(string[] args)
        {
            IPAddress ipAddress = GetIPAddress();
            int port = GetPort();

            Console.WriteLine("Команда для получения наиболее часто встречающихся слов get <prefix>");

            while (true)
            {
                var input = ConsoleExtension.CancelableReadLine();
                var prefix = GetPrefix(input);
                var words = RequestToServer.GetTopFiveWords(ipAddress, port, prefix);

                if(words.Count > 0)
                {
                    foreach (string word in words)
                        Console.WriteLine(word);
                }
            }            
        }


        /// <summary>
        /// Возвращает IP адрес из введенной пользователем строки
        /// </summary>
        private static IPAddress GetIPAddress()
        {
            while (true)
            {
                Console.Write("Введите ip адрес или hostname сервера:\n");
                var input = ConsoleExtension.CancelableReadLine();
                Console.WriteLine("Выполняется проверка доступности хоста...");
                try
                {
                    foreach (var ip in Dns.GetHostEntry(input).AddressList)
                    {
                        if (ip.AddressFamily == AddressFamily.InterNetwork && IsHostAvailable(ip))
                        {   
                            return ip;
                        }
                    }                    
                }
                catch(SocketException e)
                {
                    Console.WriteLine(e.Message);
                }                            
            }
        }

        /// <summary>
        /// Возвращает номер порта и проверяет его корректность ввода пользователем
        /// </summary>
        private static int GetPort()
        {
            while (true)
            {
                Console.Write("Введите номер порта:\n");
                var input = ConsoleExtension.CancelableReadLine();

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

        /// <summary>
        /// Получение префикса из введенной пользователем строки, в случае неверного ввода команды пользователю
        /// предлагается повторный запрос на ввод строки
        /// </summary>
        private static string GetPrefix(string input)
        {
            while (true)
            {
                input = input.ToLower();
                var content = input.Split(' ');

                if(content.Length == 2 && input.StartsWith("get "))
                {
                    return content[1];
                }
                else
                {
                    Console.WriteLine("Для отображения наиболее часто встречающихся слов, " +
                        "должна использоваться команда get <prefix>");
                    input = ConsoleExtension.CancelableReadLine();
                }
            }
        }

        /// <summary>
        /// Метод возвращает True, если хост доступен и False, если нет
        /// </summary>
        private static bool IsHostAvailable(IPAddress address)
        {
            Ping pingSender = new Ping();
            PingReply reply = pingSender.Send(address);

            if (reply.Status != IPStatus.Success)
            {
                Console.WriteLine("В данный момент хост недоступен");
                return false;
            }

            return true;
        }
    }
}
