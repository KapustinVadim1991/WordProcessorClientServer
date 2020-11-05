using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordProcessor.DataModel;
using System.Configuration;
using System.IO;
using WordProcessor.Network;
using System.Threading;

namespace WordProcessor
{
    class Program
    {
        static string firstMessage = "Команды приложения:\nсоздание словаря [путь к файлу]\nобновление словаря [путь к файлу]\nочистка словаря\n";
        static string createStr = "создание словаря";
        static string updateStr = "обновление словаря";
        static string clearStr = "очистка словаря";

        static ushort portNumber;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory", Environment.CurrentDirectory);
            //SetConnectionString();
            SetPort();

            Task.Run(()=>AsynchronousSocketListener.StartListening(portNumber));


            Console.WriteLine(firstMessage);
            
            while (true)
            {
                string input = ConsoleExtension.CancelableReadLine().ToLower();
                if (input == "" || input.All(c => char.IsWhiteSpace(c))) break;

                if(input.StartsWith(createStr))
                {
                    RequestsToDatabase.CreateDictionary(GetPath(input, createStr));
                }
                else if (input.StartsWith(updateStr))
                {
                    RequestsToDatabase.UpdateDictionary(GetPath(input, updateStr));
                }
                else if (input.StartsWith(clearStr))
                {
                    RequestsToDatabase.ClearDictionary();
                }
                else
                {
                    RequestsToDatabase.FindTopFive(input);
                }
            }            
        }
        
        private static string GetPath(string input, string command)
        {
            return input.Substring(command.Length).Trim(new char[] { ' ', '\"'});
        }

        public static void SetConnectionString()
        {
            while (true)
            {
                Console.Write("Подключиться к существующей БД (введите путь) или создать новую (нажмите Enter):");
                var input = Console.ReadLine();
                if (input == "") break;

                if (!File.Exists(input))
                {
                    Console.WriteLine("Путь к базе данных указан неверно");
                }
                else if (!input.Trim(new char[] { ' ','\"' }).EndsWith(".mdf"))
                {
                    Console.WriteLine("Расширение файла базы данных должно быть .mdf");
                }
                else
                {
                    var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    var connectionStringSection = (ConnectionStringsSection)config.GetSection("connectionStrings");
                    connectionStringSection.ConnectionStrings["DictionaryContext"].ConnectionString =
                        $"data source=(LocalDb)\\MSSQLLocalDB;AttachDbFileName={input};integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
                    config.Save();
                    ConfigurationManager.RefreshSection("connectionStrings");
                    break;
                }
            }
        }

        public static void SetPort()
        {
            while (true)
            {
                Console.Write("Введите номер порта: ");
                var input = Console.ReadLine();

                if(ushort.TryParse(input, out ushort port))
                {
                    portNumber = port;
                    break;
                }
                else
                {
                    Console.WriteLine("Неправильно указан номер порта");
                }
            }
        }
    }
}
