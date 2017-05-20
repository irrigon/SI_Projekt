using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI_Projekt{
    class Program{
        static void Main(string[] args){
            createNewWords(100000, 2);

            createPoem();
            createPoem(32, "OriginalWordsPoem.txt", "WordList5000.txt");
            
            return;
        }

        static void createNewWords(
            int amount = 10000, int level = 2, string fileName = "CreatedWords.txt") {
            Nodev2 root = new Nodev2(' ');
            root.createGraph(level);
            root.teach(level);

            string[] results = new string[10000];
            for (int i = 0; i < 10000; i++)
                results[i] = root.generateNewWord(level);

            try {
                System.IO.File.WriteAllLines(fileName, results);
            }
            catch (System.IO.IOException e) {
                Console.Error.WriteLine("Error");
            }
        }

        static void createPoem(
            int numberOfLine = 16, string poemFile = "Poem.txt",  string wordsFile = "CreatedWords.txt") {
            Poem poem = new Poem(wordsFile);
            poem.createPoem(numberOfLine, poemFile);
        }
    }
}
