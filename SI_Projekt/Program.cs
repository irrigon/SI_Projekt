using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI_Projekt{
    class Program {
        static void Main(string[] args) {
            createNewWords(10000, 2);

            createPoem();
            createPoem(32, "OriginalWordsPoem.txt", "WordList5000.txt");

            createNewWordsPolish(10000, 2);

            createPoem(32, "OriginalWordsPoemPolish.txt", "WordListPolish.txt");
            createPoem(32, "PoemPolish.txt", "CreatedWordsPolish.txt");


            return;
        }

        static void createNewWords(
            int amount = 10000, int level = 2, string fileName = "CreatedWords.txt") {
            Nodev2 root = new Nodev2(' ');
            root.createGraph(level);
            root.teach(level);

            string[] results = new string[amount];
            for (int i = 0; i < amount; i++)
                results[i] = root.generateNewWord(level);

            try {
                File.WriteAllLines(fileName, results);
            }
            catch (IOException e) {
                Console.Error.WriteLine("Error");
            }
        }

        static void createPoem(
            int numberOfLine = 16, string poemFile = "Poem.txt", string wordsFile = "CreatedWords.txt") {
            Poem poem = new Poem(wordsFile);
            poem.createPoem(numberOfLine, poemFile);
        }

        static void createNewWordsPolish(
            int amount = 10000, int level = 2, string fileName = "CreatedWordsPolish.txt") {
            NodePl root = new NodePl(' ');
            root.createGraph(level);
            root.teach(level);

            string[] result = new string[amount];
            for (int i = 0; i < amount; i++)
                result[i] = root.generateNewWord(level);

            try {
                File.WriteAllLines(fileName, result);
            }
            catch (IOException e) {
                Console.Error.WriteLine("Error!");
            }
        }
    }
}
