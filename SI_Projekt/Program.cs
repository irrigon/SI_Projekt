
using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Projekt_SI_GUI;

namespace SI_Projekt{
    class Program{
        [STAThread]
        static void Main(string[] args){
            
            // Inicjalizacja okna.
            App app = new App();
            MainWindow mainWindow = new MainWindow();

            // Nowy sylabizator.
            Sylabizator.Sylabizator sylabizator = new Sylabizator.Sylabizator();

            // Elementry do tworzenia zdań.
            SentenceNode sentenceRoot = new SentenceNode("NULL", sylabizator, mainWindow);
            sentenceRoot.teach("english_sentences.txt");
            mainWindow.setSentenceRoot(sentenceRoot);

            // The point of no return:
            app.Run(mainWindow);

            // Jednorazowe przetwarzania plików:
            //findAllEnglishSentences(100);
            //findAllPolishSentences(12);
            //cutStrangeWords();

            // Generacja słów:
            /*Nodev2 root = new Nodev2(' ');
            root.createGraph();
            root.teach();
            string[] results = new string[100];
            for (int i = 0; i < 100; i++){
                results[i] = root.generateNewWord();
                Console.WriteLine("{0}", results[i]);
            }

            Console.Write("\n");

            // Generacja zdań:
            SentenceNode sentenceRoot = new SentenceNode("NULL",sylabizator);
            //sylabizator.Model.updateLanguage(Sylabizator.SylabizatorLanguage.Polish);
            sentenceRoot.teach("english_sentences.txt");

            for (int i = 0; i < 100; i++){
                Console.WriteLine("{0}", sentenceRoot.generateNewSentence());
            }

            Console.Write("\n");

            //sentenceRoot.teachRandomWords("strange_words.txt", 50);

            // Generacja wiersszy:
            //List<string> poem = sentenceRoot.generatePoem(4, 15, 4);
            List<string> poem = sentenceRoot.generatePoem(8, 7, 2);
            //List<string> poem = sentenceRoot.generatePoem(9, 4, 3);
            //List<string> poem = sentenceRoot.generatePoem(24, 3, 2);

            Console.Write("\n\n");
            for (int i = 0; i < poem.Count; i++) {
                Console.WriteLine(poem[i]);
            }

            Console.WriteLine("\nFinished.");
            Console.ReadKey();*/

            return;
        }
    }
}
