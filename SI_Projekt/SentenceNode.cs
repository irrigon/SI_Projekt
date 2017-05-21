
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
            marked = new List<bool>();
            counters = new List<int>();

            if (word != "NULL") {
                children.Add(new SentenceNode("NULL",sylabizator));
                counters.Add(0);
                marked.Add(false);
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
        
        public List<string> generatePoem(int length, int syllables, int life) {
            // Generowanie całego wierszyka, z określoną liczbą
            // sylab w wersie, i ilością wersów. Rymy dobierane
            // są automatycznie.
            // Jan Grzywacz.

            if ((length <= 0) || (syllables <= 0)) return null;

            List<string> poem = new List<string>();
            currentRhymeWords = new Stack<string>();
            maxRhymeLife = life;
            rhymeLife = 0;

            int megaResetCounter = 0;
            int megaResetMax = 10;

            for (int i = 0; i < length; i++) {

                string verse = generateNewVerse(syllables);

                // Sytuacja awaryjna - nie udało się stworzenie wersu.
                if (verse == null) {
                    megaResetCounter++;
                    bool oneTime = true;
                    while ((((megaResetCounter >= megaResetMax)) || (oneTime)) && (i > 0)) {
                        // "Cofamy się w rozwoju", usuwając ostatni wers.
                        oneTime = false;
                        if (i != 0) {
                            Console.Write("|");
                            poem.RemoveAt(i-1);
                            i -= 1;
                        }

                        // Jeżeli jakiś rym dopiero co
                        // został stworzony, to kasujemy go.
                        currentRhymeWords.Pop();
                        if (++rhymeLife == maxRhymeLife) {
                            megaResetCounter = 0;
                            currentRhymeWords.Clear();
                            rhymeLife = 0;
                        }
                    }
                    i--;
                    if (megaResetCounter >= megaResetMax) megaResetCounter = 0;
                }
                else {
                    Console.Write("("+poem.Count+")");
                    poem.Add(verse);
                }
            }

            return poem;
        }

        protected string generateNewVerse(int maxSyllables) {
            // Generowanie jednego wersetu - podobne do generateSentence,
            // ale używa zaimportowanego sylabizatora w doborze słów.
            // Jan Grzywacz.

            int index;
            mini_repeat_counter = 0;
            repeat_counter = 0;
            big_repeat_counter = 0;

            string result;
            int syllablesLeft;
            Stack<string> previousResult;
            Stack<int> previousSyllablesLeft;

            string lastWord = null;
            string newWord = null;

            bool repeat = false;
            bool undo = false;
            string[] syllables = null;

            do {
                big_repeat_counter++;
                if (big_repeat_counter >= big_repeat_max) return null;

                touched = true;
                unmarkAll();
                repeat_counter = 0;
                repeat = false;
                undo = false;

                result = "";
                syllablesLeft = maxSyllables;

                lastWord = null;
                newWord = null;

                previousResult = new Stack<string>();
                previousSyllablesLeft = new Stack<int>();
                Stack<string> previousLastWord = new Stack<string>();
                SentenceNode child;

                while (syllablesLeft > 0) {

                    if (lastWord == null) {
                        // Znajdujemy pierwsze słowo.
                        child = randomChildWithSyllables(syllablesLeft, currentRhymeWords);
                        // Jeżeli nie znaleźliśmy nic, możemy sobie już teraz odpuścić.
                        if (child == null) return null;
                        newWord = child.word;
                    }
                    else {
                        // Znajdujemy kolejne.
                        index = findChildIndex(lastWord);
                        child = (children[index].randomChildWithSyllables(syllablesLeft, currentRhymeWords));
                        
                        if (child == null) undo = true;
                        else newWord = child.word;
                        //child = randomChildWithSyllables(syllablesLeft, currentRhyme);
                    }

                    if (undo == true) {
                        // Cofanie akcji. Usuwamy połączenie między
                        // poprzednim węzłem, a tym, który się "nie sprawdził".
                        undo = false;
                        repeat_counter++;

                        String tmp = previousLastWord.Pop();
                        if ((tmp == null) || (repeat_counter >= repeat_max)) {
                            // Jak cofnęliśmy się za daleko, to wszystko od nowa.
                            Console.Write(".");
                            repeat = true;
                            break;
                        }

                        index = findChildIndex(tmp);
                        children[index].mark(children[index].findChildIndex(lastWord));

                        lastWord = tmp;
                        syllablesLeft = previousSyllablesLeft.Pop();
                        result = previousResult.Pop();
                    }
                    else {
                        previousLastWord.Push(lastWord);
                        previousSyllablesLeft.Push(syllablesLeft);
                        previousResult.Push(result);

                        // Dodawanie nowego słowa.
                        if (newWord == "NULL") break;
                        else if (syllablesLeft < maxSyllables) result += " " + newWord;
                        else result += newWord.Capitalize();

                        // Zmiana liczby pozostałych nam sylab.
                        syllables = newWord.Syllables(sylabizator);
                        syllablesLeft -= syllables.Length;
                        lastWord = newWord;
                    }
                }

                // Finalizacja i walidacja zdania.
                double myRand = rand.NextDouble();
                result += (myRand > 0.5) ? ((myRand > 0.75) ? "?" : "!") : ".";
            } while (repeat);

            // Aktualizowanie rymu.
            if (currentRhymeWords.Count == 0) rhymeLife = maxRhymeLife;
            if (--rhymeLife == 0) currentRhymeWords.Clear();
            else currentRhymeWords.Push(newWord); //syllables[syllables.Length - 1];

            return result;
        }

        protected SentenceNode randomChildWithSyllables(int syllablesLeft, Stack<string> rhymes) {
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
            if (mini_repeat_counter < mini_repeat_max) {
                for (int i = 1; i < children.Count; i++) {
                    if ((validateWord(getWord(i), syllablesLeft, rhymes)) && (!marked[i])) {
                        newChildren.Add(children[i]);
                        newCounters.Add(counters[i]);
                        newTotal += counters[i];
                        //Console.WriteLine(children[i].word);
                    }
                }
            }

            // Jeżeli nie ma już nic, zwracamy null.
            if (newChildren.Count == 0) {
                mini_repeat_counter = 0;
                return null;
            }

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

        protected bool validateWord(string word, int syllablesLeft, Stack<string> rhymes) {
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
            foreach (string rhyme in rhymes) {
                if ((syllables.Length == syllablesLeft) && (rhyme != null)) {
                    //mini_repeat_counter++;

                    // Warunek 2.5 - słowo nie może być takie same jak rym ani zawierać się jedno w drugim.
                    if (String.Compare(rhyme.LastN(word.Length), word.LastN(rhyme.Length)) == 0) return false;

                    // Porównujemy od końca do ostatniej samogłoski.
                    int compareLength = rhyme.Length-rhyme.LastVowels();

                    string rhymeSound = rhyme.LastN(compareLength);
                    int soundLength = Math.Min(rhymeSound.Length, compareLength);
                    string wordSound = syllables[syllables.Length - 1].LastN(soundLength);

                    if (String.Compare(rhymeSound, wordSound) != 0) return false;
                }
            }

            mini_repeat_counter--;
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
                marked.Add(false);
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
                marked.Add(false);
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

        public void mark(int i) {
            marked[i] = true;
            touched = true;
        }

        public void unmarkMine() {
            for (int i = 0; i < marked.Count; i++) {
                marked[i] = false;
            }
        }

        public void unmarkAll() {
            if (!touched) return;
            for (int i = 0; i < marked.Count; i++) {
                marked[i] = false;
                touched = false;
                children[i].unmarkAll();
            }
        }

        protected Random rand = new Random();
        protected string word { get; }
        protected int total { get; set; }
        protected List<int> counters { get; set; }
        protected List<bool> marked { get; set; }
        protected List<SentenceNode> children { get; set; }

        bool touched = false;

        Sylabizator.Sylabizator sylabizator;

        protected Stack<string> currentRhymeWords;
        protected int rhymeLife;
        protected int maxRhymeLife;

        int mini_repeat_counter = 0;
        int repeat_counter = 0;
        int big_repeat_counter = 0;
        int mini_repeat_max = 50;
        int repeat_max  =100;//200;
        int big_repeat_max = 10;
    }
}
