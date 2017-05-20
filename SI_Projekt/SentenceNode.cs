
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI_Projekt
{
    class SentenceNode
    {
        // Klasa tworząca zdania ze słów, na takiej samej zasadzie jak NodeV2.
        // Jedyna różnica jest taka, że liczby i suma wszystkich słów potomnych
        // przechowywana jest w węźle, który je posiada - nie w jego dzieciach.
        // Jan Grzywacz.
        
        public SentenceNode(string word) {
            // Tworzymy pustą listę słów, która
            // będzie wypełniana podczas uczenia.
            // Jan Grzywacz.
            children = new List<SentenceNode>();
            counters = new List<int>();

            if (word != "NULL") {
                children.Add(new SentenceNode("NULL"));
                counters.Add(0);
            }

            this.word = word;
            this.total = 0;
        }

        public void teach(string path) {
            string[] sentences = System.IO.File.ReadAllLines(
                Path.Combine(Environment.CurrentDirectory, path));
            foreach (string sentence in sentences)
                teachNewSentence(sentence);

            for (int i = 0; i < children.Count; i++) {
                if (counters[i] > 0) Console.WriteLine(children[i].word + " " + counters[i] + "/" + total);
            }
        }

        public string generateNewSentence() {
            // Generowanie nowego zdania,
            // na razie bez "rytmu" ani rymów.
            // Jan Grzywacz.

            string result;

            do {
                result = "";
                int lengthOfSentence = rand.Next(10, 20);

                string lastWord = null;
                string newWord = null;

                for (int i = 0; i < lengthOfSentence; i++) {

                    if (lastWord == null) {
                        // Znajdujemy pierwsze słowo.
                        newWord = randomChild().word;
                    }
                    else {
                        // Znajdujemy kolejne.
                        int index = findChildIndex(lastWord);
                        newWord = children[index].randomChild().word;
                    }

                    // Dodawanie nowego słowa.
                    if (newWord == "NULL") break;
                    else if (i > 0) result += " " + newWord;
                    else result += newWord.Capitalize();

                    lastWord = newWord;
                }

                // Finalizacja i walidacja zdania.
                double myRand = rand.NextDouble();
                result += (myRand > 0.5) ? ((myRand > 0.75) ? "?" : "!") : ".";
            } while (!validateSentence(result));

            return result;
        }

        private bool validateSentence(string sentence) {
            // Walidacja zdania według jakichś tam zasad.
            // Jan Grzywacz.

            // Zasada 1 - musi być więcej niż 3 słowa.
            string[] words = sentence.Split(new char[] { ' ' });

            if (words.Length <= 3) return false;

            return true;
        }

        private void teachNewSentence(string sentence) {
            // Analogiczna metoda do tej z NodeV2,
            // różni się wstępną obróbką zdań.
            // Jan Grzywacz.

            // Usuwanie znaków interpunkcyjnych i dużych liter.
            sentence = sentence.ToLower();
            sentence = sentence.RemoveAll(
                new string[]{"\"", ".", ",", "!", "?", "\\", ":", "-", "_"});
            
            // Dzielenie zdania na słowa.
            string[] words = sentence.Split(new char[] {' '});

            SentenceNode lastChild = null;
            SentenceNode newChild = null;

            for (int i = 0; i < words.Length; i++) {

                if (lastChild == null) {
                    // Słowo rozpoczynające.
                    newChild = addNewWord(words[i],1);
                }
                else {
                    // Normalne słowo - dodajemy je do listy dziecka.
                    newChild = addNewWord(words[i],0);
                    lastChild.appendChild(newChild);
                }

                lastChild = newChild;
            }

            // Ostatnie słowo - dodajemy zakończenie.
            lastChild.counters[0]++;
            lastChild.total++;
        }

        private SentenceNode addNewWord(string newWord, int counterAdd) {
            // Dodawanie nowego słowa do listy,
            // i aktualizacja prawdopodobieństw.
            // Jan Grzywacz.
            total += counterAdd;

            SentenceNode child = null;

            // Sprawdzamy, czy mamy już takie słowo.
            int index = findChildIndex(newWord);

            if (index != -1) {
                // Jeżeli tak, aktualizujemy jego licznik.
                child = children[index];
                counters[index] += counterAdd;
            }
            else {
                // Jeśli nie, dodajemy nowe.
                child = new SentenceNode(newWord);
                children.Add(child);
                counters.Add(counterAdd);
            }

            return child;
        }

        private void appendChild(SentenceNode child) {
            // Dodawanie istniejącego już słowa do listy dzieci.
            // Jan Grzywacz.
            total++;

            // Sprawdzamy, czy mamy już takie słowo.
            int index = findChildIndex(child.word);

            if (index != -1) {
                // Jeżeli tak, aktualizujemy jego licznik.
                counters[index]++;
            }
            else {
                // Jeśli nie, dodajemy je.
                children.Add(child);
                counters.Add(1);
            }
        }

        private int findChildIndex(string word) {
            // Znajdujemy indeks konkretnego dziecka.
            // Jeżeli takiego nie ma, zwraca -1.
            // Jan Grzywacz.

            for (int i = 0; i < children.Count(); i++) {
                if (children[i].word == word) {
                    return i;
                }
            }

            return -1;
        }

        private SentenceNode randomChild() {
            // Znajdywanie losowego dziecka na podstawie
            // utworzonego rozkładu prawdopodobieństwa.
            // Jan Grzywacz.

            SentenceNode newChild = children[0];
            double myRand = rand.NextDouble()*total;
            double dist = 0;

            for (int i = 0; i < children.Count; i++) {
                // Tworzymy pseudo-dystrybuantę.
                dist += counters[i];
                if (myRand < dist) {
                    newChild = children[i];
                    break;
                }
            }

            //Console.WriteLine(myRand + "/" + total);
            return newChild;
        }

        private SentenceNode totallyRandomChild() {
            int index = rand.Next(0, children.Count-1);
            return children[index];
        }

        Random rand = new Random();
        private string word { get; }
        private int total { get; set; }
        private List<int> counters { get; set; }
        private List<SentenceNode> children { get; set; }
    }
}
