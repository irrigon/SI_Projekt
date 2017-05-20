using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI_Projekt{
    class Program{
        static void Main(string[] args){

            //findAllEnglishSentences();

            Nodev2 root = new Nodev2(' ');
            root.createGraph();
            root.teach();
            string[] results = new string[100];
            for (int i = 0; i < 100; i++){
                results[i] = root.generateNewWord();
                Console.WriteLine("{0}", results[i]);
            }

            Console.ReadKey();
            return;
        }

        static void findAllEnglishSentences() {
            // Jan Grzywacz.
            // Metoda do obróbki jednego, konkretnego pliku.

            string pathIn = Path.Combine(Environment.CurrentDirectory, "sentences.txt");
            string pathOut = Path.Combine(Environment.CurrentDirectory, "english_sentences.txt");
            FileStream istream = new FileStream(pathIn, FileMode.Open);
            FileStream ostream = new FileStream(pathOut, FileMode.Create);
            StreamReader reader = new StreamReader(istream);
            StreamWriter writer = new StreamWriter(ostream);
            string line;

            while ((line = reader.ReadLine()) != null) {
                // Copy only "eng" lines.
                int k = 0;
                while (isDigit(line[k])) k++;
                while (!isLetter(line[k])) k++;
                if (line.Substring(k).StartsWith("eng")) {
                    writer.WriteLine(line.Substring(k+4));
                }
            }

            Console.WriteLine("Sentence picking finished.");
        }

        static bool isDigit(char c) {
            return ((c >= '0') && (c <= '9'));
        }

        static bool isLetter(char c) {
            return (((c >= 'A') && (c <= 'Z')) || ((c >= 'a') && (c <= 'z')));
        }
    }
}
