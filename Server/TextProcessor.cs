using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordProcessor.DataModel;
using System.IO;
using System.Text.RegularExpressions;

namespace WordProcessor
{
    class TextProcessor
    {
        private string pathToFile;

        /// <summary>
        /// Чтение файла с последующим преоброзованием текста в список слов.
        /// </summary>
        /// <param name="path">Путь к файлу с текстом</param>
        /// <returns>Возвращает список Word со словами отвечающими требованиям отбора. Если файл не найден возвращается пустой список</returns>
        public List<Word> GetWordsFromFile(string path)
        {
            pathToFile = path;
            if (!IsFileExist()) return new List<Word>();

            string text = ReadFromFile();
            if (text == "" || text.All(c => char.IsWhiteSpace(c))) 
            {
                Console.WriteLine("Файл не содержит слов");
                return new List<Word>(); 
            }

            return ConvertToWords(text);
        }

        private string ReadFromFile()
        {
            Console.WriteLine("Выполняется чтение из файла...");
            string text;

            try
            {
                using (StreamReader reader = new StreamReader(File.OpenRead(pathToFile), Encoding.UTF8))
                {
                    text = reader.ReadToEnd();                    
                }
                Console.WriteLine("Чтение из файла успешно завершено.");
                return text;
            }
            catch(Exception e)
            {
                Console.WriteLine($"Возникли проблемы при чтении из файла: {e.Message}");
                return null;
            }
        }


        /// <summary>
        /// Отбор подходящих по длине и частоте появления слов c последующей их конвертацией в тип Word для последующих операций с БД
        /// </summary>
        private List<Word> ConvertToWords(string text)
        {
            Console.WriteLine("Обработка данных...");

            // Отбираем все слова из строкм
            MatchCollection words = Regex.Matches(text.ToString(), @"[А-Яа-яA-Za-z]+");        
            var dictionary = new Dictionary<string, int>();

            // Заполняем Dictionary "слово -> сколько раз встречалось в строке"
            foreach(object w in words)
            {
                var word = w.ToString().ToLower();

                if(word.Length > 2 && word.Length <= 15)
                {
                    if (dictionary.ContainsKey(word))
                    {
                        dictionary[word]++;
                    }
                    else
                    {
                        dictionary.Add(word, 1);
                    }
                }             
            }
            // Преобразуем Dictionary в List<Word>, отбрасывая слова с частотой появления в тексте меньше двух раз
            List<Word> _words = dictionary.Select(x => new Word(x.Key, x.Value)).Where(x=>x.Count > 2).ToList();
            if(_words.Count == 0)
            {
                Console.WriteLine("Файл не содержит подходящих слов");
            }
            return _words;
        }

        private bool IsFileExist()
        {
            if (!File.Exists(pathToFile))
            {
                Console.WriteLine("Неверно указан путь к файлу");
                return false;
            }
            return true;            
        }

    }
}
