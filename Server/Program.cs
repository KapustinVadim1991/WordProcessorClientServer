using System;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using WordProcessorServer.Network;

namespace WordProcessorServer
{
    class Program
    {
        static string firstMessage = "Команды приложения:\n\tсоздание словаря [путь к файлу]\n\tобновление словаря [путь к файлу]\n\tочистка словаря\n";
        static string createStr = "создание словаря";
        static string updateStr = "обновление словаря";
        static string clearStr = "очистка словаря";

        static int portNumber;

        static void Main(string[] args)
        {            
            SetConnectionString();
            SetPort();

            // Запускаем сервер в отдельном потоке, чтобы мы могли параллельно работать со словарем
            Task.Run(()=>ServerListener.StartListening(portNumber));

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
            }            
        }    

        /// <summary>
        /// Метод устанавливает в конфигурационном файле App.config строку подключения к БД
        /// </summary>
        private static void SetConnectionString()
        {
            while (true)
            {
                Console.WriteLine("Подключиться к существующей БД (введите путь) или создать новую (нажмите Enter)\n" +
                    "Если БД была автоматически создана этим приложением ранее, также нажмите Enter:");
                var input = ConsoleExtension.CancelableReadLine();
                if (input == "")
                {
                    AppDomain.CurrentDomain.SetData("DataDirectory", Environment.CurrentDirectory);
                    break;
                }

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

        /// <summary>
        /// Устанавливается и проверяется корректность ввода номера порта 
        /// </summary>
        private static void SetPort()
        {
            while (true)
            {
                Console.Write("Введите номер порта:\n");
                var input = ConsoleExtension.CancelableReadLine();
                if (input == "" || input.All(c => char.IsWhiteSpace(c))) Environment.Exit(0);

                if (ushort.TryParse(input, out ushort port))
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

        private static string GetPath(string input, string command)
        {
            return input.Substring(command.Length).Trim(new char[] { ' ', '\"' });
        }
    }
}
