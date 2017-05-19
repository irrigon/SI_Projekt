using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI_Projekt{
    class Program{
        static void Main(string[] args){
            const int level = 1;
            Nodev2 root = new Nodev2(' ');
            root.createGraph(level);
            root.teach(level);
            string[] results = new string[100];
            for (int i = 0; i < 100; i++){
                results[i] = root.generateNewWord(level);
                Console.WriteLine("{0}", results[i]);
            }
            return;
        }
    }
}
