using System;
using System.IO;
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


        public void showFreq() {
            foreach (Nodev2 child in children)
                Console.WriteLine(
                    "{0} = {1}/{2} = {3}", child.letter,
                    child.counter, child.total, (double)child.counter / child.total);
        }

        public void createGraph(int depth) {
            this.children = createChildren();
            if (depth == 0)
                return;
            foreach (Nodev2 child in this.children)
                child.createGraph(depth - 1);
        }
        

        public void teach(int level, string path = "WordList5000.txt") {
            try {
                string[] words = System.IO.File.ReadAllLines(Path.GetFullPath(path));
                
                foreach (string word in words) {
                    listOfWords.Add(word);
                    teachNewWord(word, level);
                }
            }
            catch (IOException e) {
                Console.Error.WriteLine("Can't find file");
            }
        }

        public string generateNewWord(int level) {
            string result = "";
            switch (level) {
                case 1:
                    while (result.Length < 3) {
                        result = generateNewWordLevel1();
                        if (listOfWords.Exists(x => x == result))
                            result = "";
                    }
                    break;
                case 2:
                    while (result.Length < 3) {
                        result = generateNewWordLevel2();
                        if (listOfWords.Exists(x => x == result))
                            result = "";
                    }
                    break;
            }
            listOfWords.Add(result);
            return result;
        }
        
        private string generateNewWordLevel1() {
            string result = "";
            int index, lastLetter = 0;
            int lengthOfWord = 3;// = rand.Next(3, 12);
            
            for (double prob = 0.3; prob < rand.NextDouble(); lengthOfWord++, prob += 0.08) ;
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

                if (result.Length == lengthOfWord)
                    return result;

                missesCounter = 0;
                lastLetter = index;
            }
        }

        private string generateNewWordLevel2() {
            string result = "";
            int lastLetter = 0, prelastLetter = 0, index = 0;
            int wordLength = 3;// = rand.Next(3, 12);
            for (double prob = 0.3; prob < rand.NextDouble(); wordLength++, prob += 0.08) ;
            int missesCounter = 0;

            while (true) {
                index = rand.Next(0, 25);
                if (result.Length > 1) {
                    if ((double)this.children[prelastLetter].children[lastLetter].children[index].counter / 
                        this.children[prelastLetter].children[lastLetter].counter > 0.06)
                        result += this.children[prelastLetter].children[lastLetter].children[index].letter;
                    else {
                        if (++missesCounter > 100) return result;
                        continue;
                    }
                }
                else if (result.Length == 1) {
                    if ((double)this.children[lastLetter].children[index].counter /
                        this.children[lastLetter].counter > 0.06)
                        result += this.children[lastLetter].children[index].letter;
                    else {
                        if (++missesCounter > 50) return result;
                        continue;
                    }
                }
                else
                    result += this.children[index].letter;

                if (result.Length == wordLength)
                    return result;

                missesCounter = 0;
                prelastLetter = lastLetter;
                lastLetter = index;
            }
            
        }

        private void teachNewWord(string word, int level) {
            switch (level){
                case 1:
                    teachNewWordLevel1(word);
                    break;
                case 2:
                    teachNewWordLevel2(word);
                    break;
            }
        }

        private void teachNewWordLevel1(string word) {
            int lastLetter = 0, actLetter = 0;
            int letterPointer = 0;
            while(word.Length > letterPointer) {
                for (int i = 0; i < 27; this.children[i++].total++);

                actLetter = calculateIndex(word[letterPointer]);
                if(actLetter > 26) {
                    letterPointer++;
                    continue;
                }

                if (letterPointer != 0){
                    for (int i = 0; i < 27; this.children[lastLetter].children[i++].total++);
                        
                    this.children[lastLetter].children[actLetter].counter++;
                }

                lastLetter = actLetter;
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

        private void teachNewWordLevel2(string word) {
            int lastLetter = 0, preLastLetter = 0, actLetter = 0;
            int letterPointer = 0;

            while (word.Length > letterPointer) {

                actLetter = calculateIndex(word[letterPointer]);
                if (actLetter > 26) {
                    letterPointer++;
                    continue;
                }

                for (int i = 0; i < 27; this.children[i++].total++);

                if (letterPointer > 1){
                    for (int i = 0; i < 27; 
                        this.children[preLastLetter].children[lastLetter].children[i++].total++);

                    this.children[preLastLetter].children[lastLetter].children[actLetter].counter++;
                }

                if (letterPointer != 0){
                    for (int i = 0; i < 27; this.children[lastLetter].children[i++].total++);
                        
                    this.children[lastLetter].children[actLetter].counter++;
                    
                    preLastLetter = lastLetter;
                }
                
                lastLetter = actLetter;
                this.children[lastLetter].counter++;
                letterPointer++;
            }
            
        }

        private int calculateIndex(char letter) {
            if (letter >= 'A' && letter <= 'Z')
                return letter - 0x41;
            else if (letter >= 'a' && letter <= 'z')
                return letter - 0x61;
            else if (letter == '\0')
                return 26;
            return (int)letter;
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

        private Random rand = new Random();

        private List<string> listOfWords = new List<string>();

        public char letter { get; }
        public int total { get; set; }
        public int counter { get; set; }
        public Nodev2[] children { get; set; }
    }
}
