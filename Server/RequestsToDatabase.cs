using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using WordProcessor.DataModel;
using System.Data.SqlClient;

namespace WordProcessor
{
    public static class RequestsToDatabase
    {

        /// <summary>
        /// Выборка наиболее часто встречающихся пяти слов из словаря, начало которых соответствует префиксу.
        /// </summary>
        public static void FindTopFive(string prefix)
        {
            try
            {
                using (DictionaryContext context = new DictionaryContext())
                {                    
                    if (!context.Database.Exists())
                    {
                        Console.WriteLine("Словаря не существует, воспользуйтесь командой [создание словаря]\n");
                        return;
                    }

                    var result = context.Words.Where(x => x.Text.StartsWith(prefix))
                                              .OrderByDescending(x => x.Count)
                                              .ThenBy(x => x.Text)
                                              .Take(5)
                                              .ToList();

                    foreach(Word word in result)
                    {
                        Console.WriteLine(word.Text);
                    }
                    Console.WriteLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nПри подключении к словарю возникла непредвиденная ошибка:\n{e.Message} {e.StackTrace}");
            }
        }

        /// <summary>
        /// Создание базы данных DictionaryDb и таблицы Word, с последующим заполнением элементами List<Word>, в случае, 
        /// если словарь существует, но данные в нем отсутствуют, заполняет уже имеющуюся таблицу.
        /// </summary>
        public static void CreateDictionary(string pathToFile)
        {
            try
            {
                using (DictionaryContext context = new DictionaryContext())
                {
                    if (context.Database.Exists())
                    {
                        int hasRecord = context.Words.Take(1).ToList().Count;
                        if (hasRecord != 0)
                        {
                            Console.WriteLine("Словарь уже существует и содержит слова. Вы можете обновить словарь, либо очистить существующий.\n");
                            return;
                        }
                    }
                    List<Word> words = new TextProcessor().GetWordsFromFile(pathToFile);
                    if (words.Count == 0) return;

                    CreateDictionary(context, words);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"\nПри создании словаря возникла непредвиденная ошибка:\n{e.Message} {e.StackTrace}");
            }
        }

        // Для избежания дублирования кода, 
        private static void CreateDictionary(DictionaryContext context, List<Word> words)
        {
            Console.WriteLine("Процесс создания словаря...");
            context.Words.AddRange(words);
            context.SaveChanges();
            Console.WriteLine("Словарь успешно создан!");
        }

        /// <summary>
        /// Добавление данных в Word, если база данных еще не создана, вызывается метод CreateDictionary и происходит её создание
        /// </summary>
        public static void UpdateDictionary(string pathToFile)
        {
            List<Word> words = new TextProcessor().GetWordsFromFile(pathToFile);
            if (words.Count == 0) return;

            try
            {
                using (DictionaryContext context = new DictionaryContext())
                {
                    if (!context.Database.Exists())
                    {
                        CreateDictionary(context, words);
                        return;
                    }

                    Console.WriteLine("Процесс обновления словаря...");
                    foreach (Word word in words)
                    {
                        Word dbElement = context.Words.Find(word.Text);    
                        if(dbElement != null)
                        {
                            dbElement.Count += word.Count;
                        }
                        else
                        {
                            context.Words.Add(word);           
                        }
                    }
                    context.SaveChanges();
                    Console.WriteLine("Словарь успешно обновлён!\n");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nПри обновлении словаря возникла непредвиденная ошибка:\n{e.Message} {e.StackTrace}");
            }
        }

        /// <summary>
        /// Удаление всех данных из таблицы Words
        /// </summary>
        public static void ClearDictionary()
        {
            try
            {
                using (DictionaryContext context = new DictionaryContext())
                {
                    if (!context.Database.Exists())
                    {
                        Console.WriteLine("Словарь не может быть очищен, так как его не существует!\n");
                        return;
                    }

                    Console.WriteLine("Процесс очистки словаря...");

                    context.Words.RemoveRange(context.Words);
                    context.SaveChanges();
                    Console.WriteLine("Словарь успешно очищен!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nПри очистке словаря возникла непредвиденная ошибка:\n{e.Message} {e.StackTrace}");
            }
        }
    }
}
