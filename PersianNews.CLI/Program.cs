using System;
using System.Diagnostics;
using System.Threading.Tasks;
using PersianNews.Engine;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace PersianNews.CLI
{
    class Program
    {
        #region main
        static void Main(string[] args)
        {
            var pi = new ProcessStartInfo(@"C:\Program Files\ConEmu\ConEmu\ConEmuC.exe", "/AUTOATTACH")
            {
                CreateNoWindow = false,
                UseShellExecute = false
            };
            Process.Start(pi);

            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.BackgroundColor = ConsoleColor.Black;

            //TrainFirstPart();
            //FirstPartOnTrainSet();

            ShowBanner();
        }
        #endregion main

        private static void ShowBanner()
        {
            Console.WriteLine("===========================");
            Console.WriteLine("=== GUILAN NLP ===");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("HASTI HASSANI MOUGHADAM");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("ARYAN EBRAHIMPOUR");
            Console.ResetColor();
            Console.Write("11111- Train Category Model ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("(Takes some time!)");
            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine("2- Show Result of Category Model on Test Data");
            Console.WriteLine("3- Get user input for category detection");
            Console.Write("44444- Train Agency Model ");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("(Takes some time!)");
            Console.WriteLine();
            Console.ResetColor();
            Console.WriteLine("2- Show Result of Agency Model on Test Data");
            Console.WriteLine("3- Get user input for agency detection");
            Console.WriteLine("===========================");

            Console.Write("Enter Option: ");
            int.TryParse(Console.ReadLine(), out int result);
            if (result == 11111) TrainCategoryPart();
            else if (result == 2) CategoryPartOnTrainSet();
            else if (result == 3) GetUserInputCategory();
            else if (result == 44444) TrainAgencyData();
            else if (result == 5) AgencyPartOnTrainSet();
            else if (result == 6) GetUserInputAgency();
        }

        private static void TrainCategoryPart() => DataProducer.CreateTfIdfJsons();
        private static void TrainAgencyData() => AgencyDataProducer.CreateAgencyTfIdfJsons();

        private static void CategoryPartOnTrainSet()
        {
            TfIdf.LoadDb();

            var testSet = DataLoad.AsrTestSet(DataLoad.FetchAsrDocuments);

            float correct = 0;
            float incorrect = 0;

            foreach (var document in testSet)
            {

                var (wordList, cat) = document.Item;

                var scores = new Dictionary<AsrIranCategories, double>();

                foreach (var e in Enum.GetValues(typeof(AsrIranCategories)))
                {
                    var en = (AsrIranCategories)e;
                    scores.Add(en, 0);
                }

                Console.WriteLine("Calculating...");
                foreach (var word in wordList)
                {
                    foreach (var e in Enum.GetValues(typeof(AsrIranCategories)))
                    {
                        var categorie = (AsrIranCategories)e;
                        scores[categorie] += TfIdf.CalculateTfIdf(word, categorie);
                    }
                }

                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Correct Class: " + cat);
                var orderedScore = scores.OrderByDescending(x => x.Value);
                if (orderedScore.First().Key != cat)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    incorrect++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    correct++;
                }

                foreach (var item in orderedScore.Take(5))
                {
                    Console.WriteLine($"{item.Key} = {item.Value}");
                    Console.ResetColor();
                }
                Console.WriteLine();
                Console.WriteLine();

                Console.WriteLine("===== ACURACY  : " + correct * 100 / (correct + incorrect) + "======");
            }

        }
        private static void AgencyPartOnTrainSet()
        {
            AgencyTfIdf.LoadDb();

            var testSet1 = DataLoad.FormalTestSet(DataLoad.FetchFarsFormalForm);
            var testSet2 = DataLoad.FormalTestSet(DataLoad.FetchAsrFormalForm);
            var testSet = testSet1.Concat(testSet2);

            float correct = 0;
            float incorrect = 0;

            foreach (var document in testSet)
            {

                var (wordList, cat) = document.Item;

                var scores = new Dictionary<NewsAgency, double>();

                foreach (var e in Enum.GetValues(typeof(NewsAgency)))
                {
                    var en = (NewsAgency)e;
                    scores.Add(en, 0);
                }

                Console.WriteLine("Calculating...");
                foreach (var word in wordList)
                {
                    foreach (var e in Enum.GetValues(typeof(NewsAgency)))
                    {
                        var categorie = (NewsAgency)e;
                        scores[categorie] += AgencyTfIdf.CalculateTfIdf(word, categorie);
                    }
                }

                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("Correct Class: " + cat);
                var orderedScore = scores.OrderByDescending(x => x.Value);
                if (orderedScore.First().Key != cat)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    incorrect++;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    correct++;
                }

                foreach (var item in orderedScore.Take(5))
                {
                    Console.WriteLine($"{item.Key} = {item.Value}");
                    Console.ResetColor();
                }
                Console.WriteLine();
                Console.WriteLine();

                Console.WriteLine("===== ACURACY  : " + correct * 100 / (correct + incorrect) + "======");
            }
        }

        private static void GetUserInputCategory()
        {
            TfIdf.LoadDb();

            Console.Write("Enter file path: ");
            var path = Console.ReadLine();
            Console.WriteLine();
            var text = File.ReadAllText(path);
            Console.WriteLine(text);
            var tokens = DataCleanser.CleanseUserInput(text);

            var scores = new Dictionary<AsrIranCategories, double>();

            foreach (var e in Enum.GetValues(typeof(AsrIranCategories)))
            {
                var en = (AsrIranCategories)e;
                scores.Add(en, 0);
            }

            Console.WriteLine("Calculating...");
            foreach (var word in tokens)
            {
                foreach (var e in Enum.GetValues(typeof(AsrIranCategories)))
                {
                    var categorie = (AsrIranCategories)e;
                    scores[categorie] += TfIdf.CalculateTfIdf(word, categorie);
                }
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            var orderedScore = scores.OrderByDescending(x => x.Value);

            foreach (var item in orderedScore)
            {
                Console.WriteLine($"{item.Key} = {item.Value}");
            }
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();

        }
        private static void GetUserInputAgency()
        {
            AgencyTfIdf.LoadDb();

            Console.Write("Enter file path: ");
            var path = Console.ReadLine();
            Console.WriteLine();
            var text = File.ReadAllText(path);
            Console.WriteLine(text);
            var tokens = DataCleanser.CleanseUserInput(text);

            var scores = new Dictionary<NewsAgency, double>();

            foreach (var e in Enum.GetValues(typeof(NewsAgency)))
            {
                var en = (NewsAgency)e;
                scores.Add(en, 0);
            }

            Console.WriteLine("Calculating...");
            foreach (var word in tokens)
            {
                foreach (var e in Enum.GetValues(typeof(NewsAgency)))
                {
                    var categorie = (NewsAgency)e;
                    scores[categorie] += AgencyTfIdf.CalculateTfIdf(word, categorie);
                }
            }

            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Blue;
            var orderedScore = scores.OrderByDescending(x => x.Value);

            foreach (var item in orderedScore)
            {
                Console.WriteLine($"{item.Key} = {item.Value}");
            }
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();

        }
    }
}
