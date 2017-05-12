using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI_Projekt{
    class Node {
        public Node(char letter) {
            this.counter = 0;
            this.total = 0;
            this.letter = letter;
            this.children = new Node[27];
        }

        public string generateNewWord() {
            string result = "";
            generateAnotherLetter(ref result);
            return result;
        }

        private void generateAnotherLetter(ref string result) {
            Random rand = new Random();
            while (true){
                int index = rand.Next(0, 26);
                if ((double)children[index].counter / (double)children[index].total > 0.015) {
                    result += children[index].letter;
                    if (children[index].letter == '\0' || result.Length > 20) return;
                    children[index].generateAnotherLetter(ref result);
                }
            }
        }

        public void createGraph(int depth) {
            createFirstLevel();
            createNextLevels(depth - 1);
        }

        public void teach() {
            string[] words = System.IO.File.ReadAllLines(@"C:\Users\Klient\Documents\Visual Studio 2015\Projects\SI_Projekt\SI_Projekt\WordList.txt");
            foreach (string word in words)
                teachNewWord(word,0);
        }

        private void teachNewWord(string word, int letterPointer) {
            foreach (Node child in children)
                child.total++;
            if (word.Length == letterPointer) {
                children[26].counter++;
                return;
            }
            if(word[letterPointer] >= 'A' && word[letterPointer] <= 'Z')
                children[(int)word[letterPointer] - 0x41].counter++;
            else if (word[letterPointer] >= 'a' && word[letterPointer] <= 'z')
                children[(int)word[letterPointer] - 0x61].counter++;
            children[0].teachNewWord(word, letterPointer + 1);
        }

        private void createFirstLevel() {
            this.children = createNewLevel();
        }

        private void createNextLevels(int depth) {
            if (depth == 0) return;
            Node[] children = createNewLevel();
            for (int i = 0; i < 27; i++) 
                this.children[i].children = children;
            this.children[0].createNextLevels(depth - 1);
        }
        
        private Node[] createNewLevel() {
            Node[] children = new Node[27];
            for (int i = 0; i < 27; i++) {
                if (i == 26)
                    children[i] = new Node('\0');
                else
                    children[i] = new Node((char)(0x61 + i));
                if (i != 0)
                    children[i].brother = children[i - 1];
            }
            return children;
        }


        private char letter { get; }
        private int total { get; set; }
        private int counter { get; set; }
        private Node[] children { get; set; }
        private Node brother { get; set; }
    }
}
