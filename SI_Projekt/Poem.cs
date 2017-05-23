using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI_Projekt {
    class Poem {
        public Poem(string fileInput = "CreatedWords.txt") {
            diffFromAvg = 3;
            avgLetterPerLine = 26;
            for (int i = 0; i < 12; i++) 
                dictOfWords.Add(i, new List<string>());

            addWords(fileInput);
        }

        public void createPoem(int numberOfLines, string outputFile = "Poem.txt") {
            string[] poem = new string[numberOfLines];
            for (int i = 0; i < numberOfLines; i++)
                poem[i] = createNewLine();
            try {
                File.WriteAllLines(outputFile, poem);
            }
            catch (IOException e) {
                Console.Error.WriteLine("Error");
            }
        }

        private string createNewLine() {
            List<string> tmpList;
            string result = "";
            int wordLength;
            while (true) {
                wordLength = 3;// = rand.Next(3, 12);
                for (double prob = 0.3; prob < rand.NextDouble(); wordLength++, prob += 0.08) ;
                dictOfWords.TryGetValue(wordLength, out tmpList);
                if (wordLength > avgLetterPerLine + diffFromAvg)
                    continue;
                result += tmpList[rand.Next(0, tmpList.Count)];
                result += " ";
                if (result.Length > avgLetterPerLine - diffFromAvg)
                    return result;
            }
        }

        private void addWords(string fileInput) {
            try {
                string[] words = File.ReadAllLines(Path.GetFullPath(fileInput));

                List<string> list;
                foreach (string word in words) {
                    if (!dictOfWords.TryGetValue(word.Length, out list)) {
                        list = new List<string>();
                        dictOfWords[word.Length] = list;
                    }
                    list.Add(word);
                }
            }
            catch (IOException e) {
                Console.Error.WriteLine("Error! Can't find file.");
            }
        }

        private int diffFromAvg;
        private int avgLetterPerLine;

        private Random rand = new Random();

        private Dictionary<int, List<string>> dictOfWords =
            new Dictionary<int, List<string>>(); 

    }
}
