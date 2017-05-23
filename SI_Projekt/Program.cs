using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI_Projekt{
    class Program{
        static void Main(string[] args){

            Sylabizator.Sylabizator sylabizator = new Sylabizator.Sylabizator();

            //findAllEnglishSentences(100);
            //findAllPolishSentences(12);
            //cutStrangeWords();

            Nodev2 root = new Nodev2(' ');
            root.createGraph();
            root.teach();
            string[] results = new string[100];
            for (int i = 0; i < 100; i++){
                results[i] = root.generateNewWord();
                Console.WriteLine("{0}", results[i]);
            }

            Console.Write("\n");

            SentenceNode sentenceRoot = new SentenceNode("NULL",sylabizator);
            //sylabizator.Model.updateLanguage(Sylabizator.SylabizatorLanguage.Polish);
            sentenceRoot.teach("english_sentences.txt");

            for (int i = 0; i < 100; i++){
                Console.WriteLine("{0}", sentenceRoot.generateNewSentence());
            }

            Console.Write("\n");

            //sentenceRoot.teachRandomWords("strange_words.txt", 50);

            //List<string> poem = sentenceRoot.generatePoem(4, 15, 4);
            List<string> poem = sentenceRoot.generatePoem(8, 7, 2);
            //List<string> poem = sentenceRoot.generatePoem(9, 4, 3);
            //List<string> poem = sentenceRoot.generatePoem(24, 3, 2);

            Console.Write("\n\n");
            for (int i = 0; i < poem.Count; i++) {
                Console.WriteLine(poem[i]);
            }

            Console.WriteLine("\nFinished.");
            Console.ReadKey();
            return;
        }
    }
}
