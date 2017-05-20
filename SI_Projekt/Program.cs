using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI_Projekt{
    class Program{
        static void Main(string[] args){
            const int level = 2;
            Nodev2 root = new Nodev2(' ');
            root.createGraph(level);
            root.teach(level);

            string[] results = new string[10000];
            for (int i = 0; i < 10000; i++)
                results[i] = root.generateNewWord(level);
            
            try {
                System.IO.File.WriteAllLines("CreatedWords.txt", results);
            }
            catch (System.IO.IOException e) {
                Console.Error.WriteLine("Error");
            }
            return;
        }
    }
}
