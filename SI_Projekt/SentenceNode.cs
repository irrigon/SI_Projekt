
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
        
        public SentenceNode(string word, Sylabizator.Sylabizator sylabizator) {
            // Tworzymy pustą listę słów, która
            // będzie wypełniana podczas uczenia.
            // Jan Grzywacz.

            children = new List<SentenceNode>();
            counters = new List<int>();

            if (word != "NULL") {
                children.Add(new SentenceNode("NULL",sylabizator));
                counters.Add(0);
            }

            this.sylabizator = sylabizator;
            this.word = word;
            this.total = 0;
        }

        public void teach(string path) {
            string[] sentences = System.IO.File.ReadAllLines(
                Path.Combine(Environment.CurrentDirectory, path));
            foreach (string sentence in sentences)
                teachNewSentence(sentence);

            for (int i = 0; i < children.Count; i++) {
                //if (counters[i] > 0) Console.WriteLine(children[i].word + " " + counters[i] + "/" + total);
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

        protected bool validateSentence(string sentence) {
            // Walidacja zdania według jakichś tam zasad.
            // Jan Grzywacz.

            // Zasada 1 - musi być więcej niż 3 słowa.
            string[] words = sentence.Split(new char[] { ' ' });

            if (words.Length <= 3) return false;

            return true;
        }
        
        public List<string> generatePoem(int length, int syllables) {
            // Generowanie całego wierszyka, z określoną liczbą
            // sylab w wersie, i ilością wersów. Rymy dobierane
            // są automatycznie.
            // Jan Grzywacz.

            if ((length <= 0) || (syllables <= 0)) return null;

            List<string> poem = new List<string>();

            currentRhyme = null;
            rhymeLife = 0;

            for (int i = 0; i < length; i++) {
                poem.Add(generateNewVerse(syllables));
            }

            return poem;
        }

        protected string generateNewVerse(int maxSyllables) {
            // Generowanie jednego wersetu - podobne do generateSentence,
            // ale używa zaimportowanego sylabizatora w doborze słów.
            // Jan Grzywacz.

            string result;
            bool repeat = false;
            string[] syllables = null;

            do {
                result = "";
                int syllablesLeft = maxSyllables;

                string lastWord = null;
                string newWord = null;

                while (syllablesLeft > 0) {

                    if (lastWord == null) {
                        // Znajdujemy pierwsze słowo.
                        SentenceNode child = randomChildWithSyllables(syllablesLeft,currentRhyme);

                        // Jeżeli nie znaleźliśmy nic, możemy sobie już teraz odpuścić.
                        if (child == null) return null;

                        newWord = child.word;
                    }
                    else {
                        // Znajdujemy kolejne.
                        int index = findChildIndex(lastWord);
                        SentenceNode child = (children[index].randomChildWithSyllables(syllablesLeft, currentRhyme));
                        
                        while (child == null) {
                            child = randomChildWithSyllables(syllablesLeft, currentRhyme);
                            //repeat = true;
                            //break;
                        }

                        newWord = child.word;
                    }

                    // Dodawanie nowego słowa.
                    if (newWord == "NULL") break;
                    else if (syllablesLeft < maxSyllables) result += " " + newWord;
                    else result += newWord.Capitalize();

                    // Zmiana liczby pozostałych nam sylab.
                    syllables = newWord.Syllables(sylabizator);
                    syllablesLeft -= syllables.Length;

                    lastWord = newWord;
                }

                // Finalizacja i walidacja zdania.
                double myRand = rand.NextDouble();
                result += (myRand > 0.5) ? ((myRand > 0.75) ? "?" : "!") : ".";
            } while (repeat);

            // Aktualizowanie rymu.
            if (currentRhyme == null) {
                currentRhyme = syllables[syllables.Length - 1];
                rhymeLife = 2;
            }
            
            if (--rhymeLife == 0) currentRhyme = null;

            return result;
        }

        protected SentenceNode randomChildWithSyllables(int syllablesLeft, string rhyme) {
            // Znajdywanie losowego dziecka, z dwoma ograniczeniami.
            // Nie może być w nim więcej sylab niż podana liczba,
            // i końcową sylabą musi być podana (chyba, że damy null).
            // Jan Grzywacz.

            List<SentenceNode> tmpChildren = children;
            List<int> tmpCounters = counters;
            int tmpTotal = total;

            List<SentenceNode> newChildren = new List<SentenceNode>();
            List<int> newCounters = new List<int>();
            int newTotal = 0;

            // Wybieramy możliwych kandydatów do nowej listy.
            for (int i = 1; i < children.Count; i++) {
                if (validateWord(getWord(i), syllablesLeft, rhyme)) {
                    newChildren.Add(children[i]);
                    newCounters.Add(counters[i]);
                    newTotal += counters[i];
                    //Console.WriteLine(children[i].word);
                }
            }

            // Jeżeli nie ma już nic, zwracamy null.
            if (newChildren.Count == 0) return null;

            // Zamiana naszych list...
            children = newChildren;
            counters = newCounters;
            total = newTotal;

            // Aby można było skorzystać ze starej funkcji.
            SentenceNode newChild = randomChild();

            // Przywracanie normalnych list.
            children = tmpChildren;
            counters = tmpCounters;
            total = tmpTotal;

            return newChild;
        }

        protected bool validateWord(string word, int syllablesLeft, string rhyme) {
            // Sprawdzanie warunków dla funkcji randomChildWithSyllables.
            // Jan Grzywacz.

            string[] syllables = word.Syllables(sylabizator);

            //Console.WriteLine(syllables[syllables.Length - 1] + "   " + rhyme);
            /*for (int i = 0; i < syllables.Count; i++) {
                Console.WriteLine(syllables[i]);
            }*/

            // Warunek 1 - sylab nie może być więcej niż zostało do końca wersetu.
            if (syllables.Length > syllablesLeft) return false;

            // Warunek 2 - jeśli słowo wypełni nam wers do końca,
            // to ostatnia sylaba musi być taka sama jak rym.
            if ((syllables.Length == syllablesLeft) && (rhyme != null)) {

                // Porównujemy od końca do ostatniej samogłoski.
                int compareLength = rhyme.Length-rhyme.LastVowels();

                string rhymeSound = rhyme.LastN(compareLength);
                int soundLength = Math.Min(rhymeSound.Length, compareLength);
                string wordSound = syllables[syllables.Length - 1].LastN(soundLength);

                if (String.Compare(rhymeSound, wordSound) != 0) return false;
            }

            return true;
        }

        protected void teachNewSentence(string sentence) {
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

        protected SentenceNode addNewWord(string newWord, int counterAdd) {
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
                child = new SentenceNode(newWord,sylabizator);
                children.Add(child);
                counters.Add(counterAdd);
            }

            return child;
        }

        protected void appendChild(SentenceNode child) {
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

        protected int findChildIndex(string word) {
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

        protected SentenceNode randomChild() {
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

        protected SentenceNode totallyRandomChild() {
            int index = rand.Next(0, children.Count-1);
            return children[index];
        }

        protected string getWord(int index) {
            return children[index].word;
        }
        
        public string getMyWord() {
            return word;
        }

        protected Random rand = new Random();
        protected string word { get; }
        protected int total { get; set; }
        protected List<int> counters { get; set; }
        protected List<SentenceNode> children { get; set; }

        Sylabizator.Sylabizator sylabizator;

        protected string currentRhyme;
        protected int rhymeLife;
    }
}
