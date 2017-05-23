
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Projekt_SI_GUI;

namespace SI_Projekt
{
    public class SentenceNode
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
        
        public SentenceNode
            (string word, Sylabizator.Sylabizator sylabizator, MainWindow window) : this(word, sylabizator) {
            // Uzupełniony konstruktor z oknem dialogowym.
            this.window = window;
        }

        public void teach(string path, bool absolutePath) {
            // Uczymy łańcuch zdaniami z danego pliku.
            // Jan Grzywacz.

            string[] sentences;
            if (absolutePath) sentences = System.IO.File.ReadAllLines(path);
            else sentences = System.IO.File.ReadAllLines(
                    Path.Combine(Environment.CurrentDirectory, path));

            foreach (string sentence in sentences)
                teachNewSentence(sentence);

            for (int i = 0; i < children.Count; i++) {
                //if (counters[i] > 0) Console.WriteLine(children[i].word + " " + counters[i] + "/" + total);
            }

            unlockWindow();
            updateLearnedList();
        }

        public void teachRandomWords(string path, int dispersion, int power, bool absolutePath) {
            // Uczymy łańcuch losowymi słowami z pliku.
            // Nie mają one żadnych powiązań, więc powiązania
            // zostaną stworzone - po prostu dorzucimy każde
            // słowo density razy w losowe miejsce sieci.
            // Jan Grzywacz.

            string[] sentences;
            if (absolutePath) sentences = System.IO.File.ReadAllLines(path);
            else sentences = System.IO.File.ReadAllLines(
                    Path.Combine(Environment.CurrentDirectory, path));

            foreach (string sentence in sentences)
                for (int i = 0; i < dispersion; i++)
                    teachNewSingleWord(sentence,power);

            for (int i = 0; i < children.Count; i++) {
                //if (counters[i] > 0) Console.WriteLine(children[i].word + " " + counters[i] + "/" + total);
            }

            unlockWindow();
            updateLearnedList();
        }

        public void teachRandomWords(string path, bool absolutePath) {
            teachRandomWords(path,wordDispersion,wordPower,absolutePath);
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

            stopped = false;

            if ((length <= 0) || (syllables <= 0)) return null;

            poem = new List<string>();
            currentRhymeWords = new Stack<string>();
            maxRhymeLife = life;
            rhymeLife = 0;

            // Na początku nie jest ograniczona liczba znaków.
            maxLetters = -1;

            int megaResetCounter = 0;
            int megaResetMax = 10;

            for (int i = 0; i < length; i++) {

                string verse = generateNewVerse(syllables, maxLetters);

                // Sytuacja awaryjna - nie udało się stworzenie wersu.
                if (verse == null) {
                    megaResetCounter++;
                    //megaResetCounter = megaResetMax;
                    bool oneTime = true;
                    while ((((megaResetCounter >= megaResetMax)) || (oneTime)) && (i > 0)) {
                        // "Cofamy się w rozwoju", usuwając ostatni wers.
                        oneTime = false;
                        if (i != 0) {
                            Console.Write("|");
                            poem.RemoveAt(i-1);
                            i --;
                        }

                        // Jeżeli jakiś rym dopiero co
                        // został stworzony, to kasujemy go.
                        if (currentRhymeWords.Count > 0) currentRhymeWords.Pop();
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
                    if (i == 0) maxLetters = verse.Length;
                    Console.Write("("+poem.Count+")");
                    poem.Add(verse);

                    // Wyświetlanie stanu wierszy w oknie.
                    updateWindowText(poem,"");
                }

                if (stopped) break;
            }

            unlockWindow();
            return poem;
        }

        public List<string> generatePoem(int length, bool isPolish)
        {
            // Do uruchamiania z WPF-a.
            polishRhymes = isPolish;

            if (isPolish) sylabizator.Model.updateLanguage(Sylabizator.SylabizatorLanguage.Polish);
            else sylabizator.Model.updateLanguage(Sylabizator.SylabizatorLanguage.English);

            return generatePoem(length,syllablesInVerse,maxRhymeLife);
        }

        protected string generateNewVerse(int maxSyllables, int maxLetters) {
            // Generowanie jednego wersetu - podobne do generateSentence,
            // ale używa zaimportowanego sylabizatora w doborze słów.
            // Jan Grzywacz.

            int index;
            mini_repeat_counter = 0;
            repeat_counter = 0;
            big_repeat_counter = 0;
            
            int syllablesLeft;
            int lettersLeft;

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
                lettersLeft = maxLetters;

                lastWord = null;
                newWord = null;

                previousResult = new Stack<string>();
                previousSyllablesLeft = new Stack<int>();
                Stack<string> previousLastWord = new Stack<string>();
                SentenceNode child;

                while (syllablesLeft > 0) {

                    if (lastWord == null) {
                        // Znajdujemy pierwsze słowo.
                        child = randomChildWithSyllables(
                            syllablesLeft, result.Length, maxLetters, currentRhymeWords);
                        // Jeżeli nie znaleźliśmy nic, możemy sobie już teraz odpuścić.
                        if (child == null) return null;
                        newWord = child.word;
                    }
                    else {
                        if ((mini_repeat_max > 0) && (mini_repeat_counter == mini_repeat_max)) {
                            mini_repeat_counter = 0;
                            undo = true;
                        }
                        else {
                            // Znajdujemy kolejne.
                            index = findChildIndex(lastWord);
                            child = (children[index].randomChildWithSyllables(
                                syllablesLeft, result.Length, maxLetters, currentRhymeWords));

                            if (child == null) undo = true;
                            else newWord = child.word;
                            //child = randomChildWithSyllables(syllablesLeft, currentRhyme);
                        }
                    }

                    if (undo == true) {
                        // Cofanie akcji. Usuwamy połączenie między
                        // poprzednim węzłem, a tym, który się "nie sprawdził".
                        undo = false;
                        mini_repeat_counter++;
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

                        // Zmiana liczby pozostałych nam sylab oraz liter.
                        syllables = newWord.Syllables(sylabizator);

                        lettersLeft -= newWord.Length+1;
                        syllablesLeft -= syllables.Length;
                        lastWord = newWord;
                    }

                    // Wyświetlanie stanu wierszy w oknie.
                    updateWindowText(poem,result);
                    if (stopped) break;
                }

                // Finalizacja i walidacja zdania.
                double myRand = rand.NextDouble();
                result += (myRand > 0.5) ? ((myRand > 0.75) ? "?" : "!") : ".";
                if (stopped) break;
            } while (repeat);

            // Aktualizowanie rymu.
            if (currentRhymeWords.Count == 0) rhymeLife = maxRhymeLife;
            if (--rhymeLife == 0) currentRhymeWords.Clear();
            else currentRhymeWords.Push(newWord); //syllables[syllables.Length - 1];

            return result;
        }

        protected SentenceNode randomChildWithSyllables(int syllablesLeft, int lTotal, int maxL, Stack<string> rhymes){
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
                if ((validateWord(getWord(i), syllablesLeft, lTotal, maxL, rhymes)) && (!marked[i])) {
                    newChildren.Add(children[i]);
                    newCounters.Add(counters[i]);
                    newTotal += counters[i];
                    //Console.WriteLine(children[i].word);
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

        protected bool validateWord(string word, int syllablesLeft, int lettersTotal, int maxLetters, Stack<string> rhymes) {
            // Sprawdzanie warunków dla funkcji randomChildWithSyllables.
            // Jan Grzywacz.

            string[] syllables = word.Syllables(sylabizator);

            //Console.WriteLine(syllables[syllables.Length - 1] + "   " + rhyme);
            /*for (int i = 0; i < syllables.Count; i++) {
                Console.WriteLine(syllables[i]);
            }*/

            // Warunek 1 - sylab nie może być więcej niż zostało do końca wersetu.
            if ((checkSyllables) && (syllables.Length > syllablesLeft)) return false;

            // Jeśli słowo wypełni nam wers do końca...
            if ((syllables.Length >= syllablesLeft)) {
                    
                // Warunek 2 - wersy nie mogą różnić się długością o więcej niż verseThreshold.
                if ((checkMaxLetters) && (maxLetters > 0)
                    && (Math.Abs(lettersTotal + word.Length + 1 - maxLetters) > verseThreshold)) return false;
                //if ((maxLetters >= 0) && (Math.Abs(word.Length-lettersLeft) > verseThreshold))return false;
                //Console.WriteLine(lettersTotal + " " + word + " " + word.Length + " " + maxLetters);

                if ((checkRhyme) && (rhymes != null)) {
                    foreach (string rhyme in rhymes) {
                        // Warunek 3 - słowo nie może być takie same jak rym ani zawierać się jedno w drugim.
                        if ((checkIdentical) && (String.Compare(rhyme.LastN(word.Length), word.LastN(rhyme.Length)) == 0)) return false;

                        // Porównujemy od końca do ostatniej samogłoski.
                        int rhymeSoundLength = rhyme.Length - (polishRhymes ? rhyme.LastVowels() : rhyme.LastVowelsWeak());
                        int wordSoundLength = word.Length - (polishRhymes ? word.LastVowels() : word.LastVowelsWeak());
                        int finalLength = Math.Max(rhymeSoundLength, wordSoundLength);

                        string rhymeSound = rhyme.LastN(finalLength);
                        string wordSound = word.LastN(finalLength);

                        // Warunek 4 - ostatnia sylaba musi być taka sama jak rym.
                        if ((String.Compare(rhymeSound, wordSound) != 0)) return false;
                    }
                }
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

            addToWindowText(sentence);
        }
        
        protected void teachNewSingleWord(string word, int power) {
            // Wrzucamy do gotowej sieci słowo w losowe miejsce.
            // Jan Grzywacz.
            
            SentenceNode victim = totallyRandomChild();
            if (victim.word == "NULL") victim = this;
            
            SentenceNode newChild = addNewWord(word, 0);
            victim.appendChild(newChild);

            SentenceNode anotherVictim = totallyRandomChild();
            newChild.appendChild(anotherVictim);

            addToWindowText(word);
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

        protected void appendChild(SentenceNode child, int power)
        {
            // Dodawanie istniejącego już słowa do listy dzieci.
            // Jan Grzywacz.
            total += power;

            // Sprawdzamy, czy mamy już takie słowo.
            int index = findChildIndex(child.word);

            if (index != -1)
            {
                // Jeżeli tak, aktualizujemy jego licznik.
                counters[index] += power;
            }
            else
            {
                // Jeśli nie, dodajemy je.
                children.Add(child);
                counters.Add(power);
                marked.Add(false);
            }
        }

        protected void appendChild(SentenceNode child) {
            appendChild(child,1);
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
        
        protected string joinStrings(string str1, string str2) {
            return str1 + "\n" + str2;
        }

        MainWindow window;

        public void stop() { stopped = true; }

        protected void updateWindowText(List<string> text, string tmp) {
            if ((window != null) && (text.Count > 0))
                window.ContentTextBoxPoem.Dispatcher.Invoke(
                    new MainWindow.UpdateTextCallback(window.updatePoem),
                        new object[] { text.Aggregate(joinStrings)+"\n"+tmp });
        }

        protected void clearWindowText()
        {
            if (window != null)
                window.ContentTextBoxPoem.Dispatcher.Invoke(
                    new MainWindow.UpdateVoidCallback(window.clearPoem),
                        new object[] {  });
        }

        protected void addToWindowText(string tmp)
        {
            if (window != null)
                window.ContentTextBoxPoem.Dispatcher.Invoke(
                    new MainWindow.UpdateTextCallback(window.addToPoem),
                        new object[] { tmp + "\n" });
        }

        protected void unlockWindow() {
            if (window != null)
                window.ContentTextBoxPoem.Dispatcher.Invoke(
                    new MainWindow.UpdateVoidCallback(window.unlockEverything),
                        new object[] {  });
            if (window != null)
                window.ContentTextBoxPoem.Dispatcher.Invoke(
                    new MainWindow.UpdateVoidCallback(window.stopTimer),
                        new object[] { });
        }

        protected void updateLearnedList()
        {
            if (window != null)
                window.ContentTextBoxPoem.Dispatcher.Invoke(
                    new MainWindow.UpdateVoidCallback(window.updatePoemFileList),
                        new object[] { });
        }

        protected static bool checkSyllables = true;
        protected static bool checkMaxLetters = true;
        protected static bool checkIdentical = true;
        protected static bool checkRhyme = true;

        public void setChecks(bool syl, bool let, bool rhyme, bool ident) {
            checkSyllables = syl;
            checkMaxLetters = let;
            checkIdentical = ident;
            checkRhyme = rhyme;
            Console.WriteLine(checkRhyme);
        }

        protected Random rand = new Random();
        protected string word { get; }
        protected int total { get; set; }
        protected List<int> counters { get; set; }
        protected List<bool> marked { get; set; }
        protected List<SentenceNode> children { get; set; }

        List<string> poem;

        bool touched = false;

        Sylabizator.Sylabizator sylabizator;

        protected Stack<string> currentRhymeWords;
        protected int maxLetters;
        protected int syllablesInVerse = 7;
        protected int maxRhymeLife = 2;
        int verseThreshold = 1;

        int wordDispersion = 30;
        int wordPower = 10;

        protected int rhymeLife;
        
        string result;

        bool stopped;
        bool polishRhymes;

        protected int mini_repeat_counter = 0;
        protected int repeat_counter = 0 ;
        protected int big_repeat_counter = 0;
        protected int mini_repeat_max = 5;//10 LESS = MORE CREATIVE RHYMES!
        protected int repeat_max = 100;//200;
        protected int big_repeat_max = 10;

        public int getSyllablesInVerse() { return syllablesInVerse; }
        public int getRhymeLife() { return maxRhymeLife; }
        public int getVerseThreshold() { return verseThreshold; }
        public int getMiniRepeatMax() { return mini_repeat_max; }
        public int getRepeatMax() { return repeat_max; }
        public int getBigRepeatMax() { return big_repeat_max; }

        public void setSyllablesInVerse(int x) { syllablesInVerse = x; }
        public void setRhymeLife(int x) { maxRhymeLife = x; }
        public void setVerseThreshold(int x) { verseThreshold = x; }
        public void setMiniRepeatMax(int x) { mini_repeat_max = x; }
        public void setRepeatMax(int x) { repeat_max = x; }
        public void setBigRepeatMax(int x) { big_repeat_max = x; }

        public int getWordDispersion() { return wordDispersion; }
        public int getWordPower() { return wordPower; }
        public void setWordDispersion(int x) { wordDispersion = x; }
        public void setWordPower(int x) { wordPower = x; }

        public Sylabizator.Sylabizator getSylabizator() { return sylabizator; }
    }
}
