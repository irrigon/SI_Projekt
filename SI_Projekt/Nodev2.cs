using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SI_Projekt
{
    class Nodev2
    {

        public Nodev2(char letter) {
            this.letter = letter;
            this.total = 0;
            this.counter = 0;
            this.children = new Nodev2[27];
        }

        public void createGraph() {
            this.children = createChildren();
            foreach (Nodev2 child in this.children)
                child.children = createChildren();
        }

        public void teach() {
            string[] words = System.IO.File.ReadAllLines(
                @"D:\School\2017_SI\SI_Projekt\SI_Projekt\WordList.txt");
            foreach (string word in words)
                teachNewWord(word, 0);
        }

        public string generateNewWord() {
            string result = "";
            int index, lastLetter = 0;
            int lengthOfWord = rand.Next(3, 12);
            int missesCounter = 0;
            while (true) {
                index = rand.Next(0, 25);
                if (result != "") {
                    if ((double)children[lastLetter].children[index].counter / children[lastLetter].counter > 0.06)
                        result += children[lastLetter].children[index].letter;
                    else {
                        if(++missesCounter > 50) return result;
                        continue;
                    }
                }
                else
                    result += children[index].letter;
                missesCounter = 0;
                lastLetter = index;
                if (result.Length == lengthOfWord)
                    return result;
            }
        }

        private void teachNewWord(string word, int letterPointer) {
            int lastLetter = 0;
            while(word.Length > letterPointer) {
                for (int i = 0; i < 27; i++)
                    this.children[i].total++;

                if (letterPointer != 0){
                    for (int i = 0; i < 27; i++)
                        this.children[lastLetter].children[i].total++;
                    if (word[letterPointer] >= 'A' && word[letterPointer] <= 'Z')
                        this.children[lastLetter].children[word[letterPointer] - 0x41].counter++;
                    else if (word[letterPointer] >= 'a' && word[letterPointer] <= 'z')
                        this.children[lastLetter].children[word[letterPointer] - 0x61].counter++;
                }

                if (word[letterPointer] >= 'A' && word[letterPointer] <= 'Z')
                    lastLetter = word[letterPointer] - 0x41;
                else if (word[letterPointer] >= 'a' && word[letterPointer] <= 'z')
                    lastLetter = word[letterPointer] - 0x61;

                this.children[lastLetter].counter++;

                letterPointer++;
            }

            for (int i = 0; i < 27; i++){
                this.children[i].total++;
                this.children[lastLetter].children[i].total++;
            }

            this.children[26].counter++;
            this.children[lastLetter].children[26].counter++;
        }

        private Nodev2[] createChildren() {
            Nodev2[] children = new Nodev2[27];
            for (int i = 0; i < 27; i++) {
                if (i == 26)
                    children[i] = new Nodev2('\0');
                else
                    children[i] = new Nodev2((char)(i + 0x61));
            }
            return children;
        }
        
        Random rand = new Random();
        private char letter { get; }
        private int total { get; set; }
        private int counter { get; set; }
        private Nodev2[] children { get; set; }
    }
}
