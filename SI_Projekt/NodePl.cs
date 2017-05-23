using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI_Projekt {
    class NodePl {
        public NodePl(char letter) {
            total = 0;
            counter = 0;
            this.letter = letter;
            children = new NodePl[36];
        }

        public void showFreq() {
            foreach (NodePl child in children)
                Console.WriteLine(
                    "{0} = {1}/{2} = {3}", child.letter,
                    child.counter, child.total, (double)child.counter / child.total);
        }

        public void createGraph(int depth) {
            this.children = createChildren();
            if (depth == 0)
                return;
            foreach (NodePl child in children)
                child.createGraph(depth - 1);
        }

        public void teach(int level, string source = "WordListPolish.txt") {
            try {
                string[] words = File.ReadAllLines(source);

                foreach (string word in words) {
                    wordsList.Add(word);
                    teachNewWord(word, level);
                }
            }
            catch (IOException e) {
                Console.Error.WriteLine("Error! File not found");
            }
        }

        public string generateNewWord(int level) {
            string result = "";
            switch (level) {
                case 1:
                    while (result.Length < 3) {
                        result = generateNewWordLevel1();
                        if (wordsList.Exists(x => x == result))
                            result = "";
                    }
                    break;
                case 2:
                    while (result.Length < 3) {
                        result = generateNewWordLevel2();
                        if (wordsList.Exists(x => x == result))
                            result = " ";
                    }
                    break;
            }

            wordsList.Add(result);
            return result;
        }

        private string generateNewWordLevel1() {
            string result = "";
            int actLetter = 0, lastLetter = 0;
            int wordLength = 3;
            for (double prob = 0.3; rand.NextDouble() > prob; wordLength++, prob += 0.08) ;
            int missesCounter = 0;

            while (true) {
                actLetter = rand.Next(0, 35);
                if (children[actLetter].counter == 0)
                    continue;
                if (result != "") {
                    if ((double)children[lastLetter].children[actLetter].counter / children[lastLetter].counter > 0.06)
                        result += children[lastLetter].children[actLetter].letter;
                    else {
                        if (++missesCounter > 50) return result;
                        continue;
                    }
                }
                else 
                    result += children[actLetter].letter;

                if (result.Length == wordLength)
                    return result;

                missesCounter = 0;
                lastLetter = actLetter;
            }
        }

        private string generateNewWordLevel2() {
            string result = "";
            int lastLetter = 0, preLastLetter = 0, actLetter = 0;
            int wordLength = 3;
            for (double prob = 0.3; prob < rand.NextDouble(); wordLength++, prob += 0.08) ;
            int missesCounter = 0;

            while (true) {
                actLetter = rand.Next(0, 35);
                if (children[actLetter].counter == 0)
                    continue;

                if (result.Length > 1) {
                    if ((double)children[preLastLetter].children[lastLetter].children[actLetter].counter /
                        children[preLastLetter].children[lastLetter].counter > 0.06)
                        result += children[preLastLetter].children[lastLetter].children[actLetter].letter;
                    else {
                        if (++missesCounter > 100)
                            return result;
                        continue;
                    }
                }
                else if (result.Length == 1) {
                    if ((double)children[lastLetter].children[actLetter].counter /
                        children[lastLetter].counter > 0.06)
                        result += children[lastLetter].children[actLetter].letter;
                    else {
                        if (++missesCounter > 50)
                            return result;
                        continue;
                    }
                }
                else
                    result += children[actLetter].letter;

                if (result.Length == wordLength)
                    return result;

                missesCounter = 0;
                preLastLetter = lastLetter;
                lastLetter = actLetter;
            }
        }

        private void teachNewWord(string word, int level) {
            switch (level) {
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

            while (word.Length > letterPointer) {
                for (int i = 0; i < 36; children[i++].total++) ;

                actLetter = calculateIndex(word[letterPointer]);
                if (actLetter > 35) {
                    letterPointer++;
                    continue;
                }

                if (letterPointer != 0) {
                    for (int i = 0; i < 36; children[lastLetter].children[i++].total++) ;

                    children[lastLetter].children[actLetter].counter++;
                }

                lastLetter = actLetter;
                children[actLetter].counter++;

                letterPointer++;
            }
        }

        private void teachNewWordLevel2(string word) {
            int preLastLetter = 0, lastLetter = 0, actLetter = 0;
            int letterPointer = 0;
            while (word.Length > letterPointer) {
                for (int i = 0; i < 36; this.children[i++].total++) ;

                actLetter = calculateIndex(word[letterPointer]);
                if(actLetter > 35) {
                    letterPointer++;
                    continue;
                }

                if (letterPointer > 1) {
                    for (int i = 0; i < 36;
                        children[preLastLetter].children[lastLetter].children[i++].total++);

                    children[preLastLetter].children[lastLetter].children[actLetter].counter++;
                }

                if (letterPointer != 0) {
                    for (int i = 0; i < 36; children[lastLetter].children[i++].total++) ;

                    children[lastLetter].children[actLetter].counter++;

                    preLastLetter = lastLetter;
                }

                lastLetter = actLetter;
                this.children[actLetter].counter++;

                letterPointer++;
            }
        }

        private int calculateIndex(char letter) {
            if (letter == 'Ą' || letter == 'ą')
                return 26;
            else if (letter == 'Ć' || letter == 'ć')
                return 27;
            else if (letter == 'Ę' || letter == 'ę')
                return 28;
            else if (letter == 'Ł' || letter == 'ł')
                return 29;
            else if (letter == 'Ń' || letter == 'ń')
                return 30;
            else if (letter == 'Ó' || letter == 'ó')
                return 31;
            else if (letter == 'Ś' || letter == 'ś')
                return 32;
            else if (letter == 'Ż' || letter == 'ż')
                return 33;
            else if (letter == 'Ź' || letter == 'ź')
                return 34;
            else if (letter == '\0')
                return 35;
            else if (letter >= 'A' && letter <= 'Z')
                return letter - 0x41;
            else if (letter >= 'a' && letter <= 'z')
                return letter - 0x61;
            return (int)letter;
        }

        private NodePl[] createChildren() {
            NodePl[] result = new NodePl[36];

            for (int i = 0; i < 36; i++) {
                switch (i) {
                    case 26:
                        result[i] = new NodePl('ą');
                        break;
                    case 27:
                        result[i] = new NodePl('ć');
                        break;
                    case 28:
                        result[i] = new NodePl('ę');
                        break;
                    case 29:
                        result[i] = new NodePl('ł');
                        break;
                    case 30:
                        result[i] = new NodePl('ń');
                        break;
                    case 31:
                        result[i] = new NodePl('ó');
                        break;
                    case 32:
                        result[i] = new NodePl('ś');
                        break;
                    case 33:
                        result[i] = new NodePl('ż');
                        break;
                    case 34:
                        result[i] = new NodePl('ź');
                        break;
                    case 35:
                        result[i] = new NodePl('\0');
                        break;
                    default:
                        result[i] = new NodePl((char)(0x61 + i));
                        break;
                }
            }
            return result;
        }

        private Random rand = new Random();

        private List<string> wordsList = new List<string>();

        public char letter { get; }
        public int total { get; set; }
        public int counter { get; set; }
        public NodePl[] children { get; set; }
    }
}
