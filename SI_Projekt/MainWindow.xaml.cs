
using System;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Xceed.Wpf.Toolkit;

using SI_Projekt;

namespace Projekt_SI_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            poemButton.IsEnabled = false;
            poemButtonStop.IsEnabled = false;
            PoemReset.IsEnabled = false;

            verseChecker.IsChecked = true;
            letterChecker.IsChecked = true;
            identChecker.IsChecked = true;
            rhymeChecker.IsChecked = true;

            //ContentTextBox.Text = "do tego pola przypisujcie wygenerowane słowa";
            //ContentTextBoxPoem.Text = "do tego pola przypisujcie wygenerowane wiersze";
        }

        public delegate void UpdateTextCallback(string message);
        public delegate void UpdateVoidCallback();

        public void startTimer()
        {
            timerStart = DateTime.Now;
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            dispatcherTimer.Start();
        }

        public void stopTimer()
        {
            dispatcherTimer.Stop();
        }

        public void clearPoem()
        { ContentTextBoxPoem.Text = ""; }

        public void addToPoem(string str)
        {
            if (toTextBoxPoem.IsChecked == true)
                ContentTextBoxPoem.AppendText(str);
            ContentTextBoxPoem.ScrollToEnd();
        }

        public void updatePoem(string str)
        {
            if (toTextBoxPoem.IsChecked == true)
                ContentTextBoxPoem.Text = str;
        }

        public void updatePoemFileList()
        {
            if (loadedPoemFiles.Count == 0) PoemLearnedBox.Text = "";
            else PoemLearnedBox.Text = loadedPoemFiles.Aggregate(joinStrings);
        }

        protected string joinStrings(string str1, string str2)
        {
            return str1 + "\n" + str2;
        }

        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        DateTime timerStart;
        private void generatePoem(object sender, RoutedEventArgs e)
        {
            lockEverything();
            startTimer();

            Console.WriteLine((bool)rhymeChecker.IsChecked);
            sentenceRoot.setChecks( (bool)verseChecker.IsChecked,
                                    (bool)letterChecker.IsChecked,
                                    (bool)rhymeChecker.IsChecked,
                                    (bool)identChecker.IsChecked );

            int x;
            Int32.TryParse(VerseAmout.Text, out x);
            if ((toFilePoem.IsChecked == true) && (dPoemSelectedPath.Length > 0))
                Task.Run(() => sentenceRoot.generatePoem(x, polish, dPoemSelectedPath));
            else Task.Run(() => sentenceRoot.generatePoem(x, polish));

            //sentenceRoot.generatePoem(8, 7, 2);
            //await Task.Factory.StartNew(() => sentenceRoot.generatePoem(8, 7, 2));
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            timerPoem.Text = (DateTime.Now.Subtract(timerStart)).ToString("mm\\:ss\\:ff");
        }

        private void stopPoem(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
            unlockEverything();
            sentenceRoot.stop();
        }

        private void generateWords(object sender, RoutedEventArgs e)
        {
            lockEverythingInWords();
        }

        private void stopWords(object sender, RoutedEventArgs e)
        {
            unlockEverythingInWords();
        }

        public void lockEverything() {

            poemButtonStop.IsEnabled = true;

            poemButton.IsEnabled = false;
            optionsButton.IsEnabled = false;
            destPoem.IsEnabled = false;
            srcPoem.IsEnabled = false;
            VerseAmout.IsEnabled = false;
            wordTab.IsEnabled = false;
            poemTab.IsEnabled = false;
            toTextBoxPoem.IsEnabled = false;
            toFilePoem.IsEnabled = false;
            PoemReset.IsEnabled = false;
            loadPoemFile.IsEnabled = false;
            SentencePoem.IsEnabled = false;
            WordPoem.IsEnabled = false;

            EnglishPoem.IsEnabled = false;
            PolishPoem.IsEnabled = false;

            toFilePoem.IsEnabled = false;
            toTextBoxPoem.IsEnabled = false;
            verseChecker.IsEnabled = false;
            letterChecker.IsEnabled = false;
            identChecker.IsEnabled = false;
            rhymeChecker.IsEnabled = false;
        }

        public void unlockEverything() {

            poemButtonStop.IsEnabled = false;

            poemButton.IsEnabled = true;
            optionsButton.IsEnabled = true;
            destPoem.IsEnabled = true;
            srcPoem.IsEnabled = true;
            VerseAmout.IsEnabled = true;
            wordTab.IsEnabled = true;
            poemTab.IsEnabled = true;
            toTextBoxPoem.IsEnabled = true;
            toFilePoem.IsEnabled = true;
            PoemReset.IsEnabled = true;
            loadPoemFile.IsEnabled = true;
            SentencePoem.IsEnabled = true;
            WordPoem.IsEnabled = true;

            EnglishPoem.IsEnabled = true;
            PolishPoem.IsEnabled = true;

            toFilePoem.IsEnabled = true;
            toTextBoxPoem.IsEnabled = true;
            verseChecker.IsEnabled = true;
            letterChecker.IsEnabled = true;
            identChecker.IsEnabled = true;
            rhymeChecker.IsEnabled = true;
        }

        public void lockEverythingInWords()
        {
            wordButtonStop.IsEnabled = true;
            wordButton.IsEnabled = false;

            src.IsEnabled = false;
            dest.IsEnabled = false;
            toFile.IsEnabled = false;
            WordAmout.IsEnabled = false;
            toTextBox.IsEnabled = false;
            englishWordsRadio.IsEnabled = false;
            polishWordsRadio.IsEnabled = false;
            wordTab.IsEnabled = false;
            poemTab.IsEnabled = false;
        }

        public void unlockEverythingInWords()
        {
            wordButtonStop.IsEnabled = false;
            wordButton.IsEnabled = true;

            src.IsEnabled = true;
            dest.IsEnabled = true;
            toFile.IsEnabled = true;
            WordAmout.IsEnabled = true;
            toTextBox.IsEnabled = true;
            englishWordsRadio.IsEnabled = true;
            polishWordsRadio.IsEnabled = true;
            wordTab.IsEnabled = true;
            poemTab.IsEnabled = true;
        }

        private void Button_Chose(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn.Name.ToString() == "src")
            {
                var dlg = new System.Windows.Forms.OpenFileDialog() { Filter = "TXT files|*.txt" };
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    sSelectedPath = dlg.FileName;
                    sFilePath.Text = sSelectedPath;
                }
            }
            else if (btn.Name.ToString() == "dest")
            {
                var dlg = new System.Windows.Forms.SaveFileDialog() { Filter = "TXT files|*.txt" };
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    dSelectedPath = dlg.FileName;
                    dFilePath.Text = dSelectedPath;
                }
            }
        }

        private void RadioButton_Language(object sender, RoutedEventArgs e){
            RadioButton rdo = sender as RadioButton;
            if (rdo.Name.ToString() == "Polish") {
                polish = true;
            }
            else if (rdo.Name.ToString() == "English") {
                polish = false;
            }
            else if (rdo.Name.ToString() == "PolishpolishWordsRadio") {
                polishWords = true;
            }
            else if (rdo.Name.ToString() == "englishWordsRadio") {
                polishWords = false;
            }
        }
        
        private void RadioButton_Poem_File(object sender, RoutedEventArgs e)
        {
            RadioButton rdo = sender as RadioButton;
            if (rdo.Name.ToString() == "SentencePoem") {
                sentences = true;
                Console.WriteLine("TEST");
            }
            else if (rdo.Name.ToString() == "WordPoem"){
                sentences = false;
                Console.WriteLine("TEST");
            }
        }


        private void getAmout_TextChanged(object sender, TextChangedEventArgs e){
            TextBox txb = sender as TextBox;
            if (txb.Name.ToString() == "WordAmout"){
                Int32.TryParse(WordAmout.Text, out wordAmount);
            }
            else if (txb.Name.ToString() == "VerseAmout") {
                Int32.TryParse(VerseAmout.Text, out verseAmount);
            }

            //System.Console.WriteLine("wordAmount = " + wordAmount);
        }

        Task writePoem;

        private int wordAmount = 0;   // z tąd bierzcie liczbę słów do wygeneroeania
        private string sSelectedPath; // ścieżka do pliku źródłowego
        private bool polish = true;   // zmienna w której jest mówi o tym jaki język został wybrany
                                      // true = polski, false = angielski
        private string dSelectedPath; // ścieżka do pliku wyjściowego
        private bool writeToFile = false;
        private bool writeToScreen = false;

        //-----------------------------------------

        private int verseAmount = 0;
        private string sPoemSelectedPath;
        private bool polishPoem = true;
        private bool polishWords = true;
        private string dPoemSelectedPath;
        private string dPoemSelectedPath2;
        private bool writePoemToFile = false;
        private bool writePoemToScreen = false;

        private bool sentences = true;

        List<string> loadedPoemFiles = new List<string>();

        private void CheckBox_Destination(object sender, RoutedEventArgs e){
            CheckBox chx = sender as CheckBox;
            if (chx.Name.ToString() == "toTextBox") {
                writeToScreen = true;
            }
            else if (chx.Name.ToString() == "toFile"){
                writeToFile = true;
            }
        }

        private void UnchckedDestiantion(object sender, RoutedEventArgs e)
        {
            CheckBox chx = sender as CheckBox;
            if (chx.Name.ToString() == "toTextBox")
            {
                writeToScreen = false;
            }
            else if (chx.Name.ToString() == "toFile")
            {
                writeToFile = false;
            }
        }

        private void Button_Chose_Poem_Source(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var dlg = new System.Windows.Forms.OpenFileDialog() {
                Filter = "TXT files|*.txt"
            };

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                sPoemSelectedPath = dlg.FileName;
                sPoemFilePath.Text = dlg.FileName;
            }
        }
        
        private void Button_Chose_Poem_Dest(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var dlg = new System.Windows.Forms.SaveFileDialog() {
                Filter = "TXT files|*.txt"
            };

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                dPoemSelectedPath = dlg.FileName;
                dPoemFilePath.Text = dlg.FileName;
                /*else if (btn.Name.ToString() == "destPoem2") {
                    dPoemSelectedPath2 = dlg.SelectedPath;
                    dPoemFilePath2.Text = dlg.SelectedPath;
                }*/
            }
        }

        private void LoadPoemFile(object sender, RoutedEventArgs e)
        {
            if (sentences) Task.Run(() => sentenceRoot.teach(sPoemSelectedPath, true));
            else Task.Run(() => sentenceRoot.teachRandomWords(sPoemSelectedPath, true));

            loadedPoemFiles.Add(System.IO.Path.GetFileName(sPoemSelectedPath));
            ContentTextBoxPoem.Text = "";

            lockEverything();
            poemButtonStop.IsEnabled = false;
            startTimer();
        }

        private void CheckBox_DestinationPoem(object sender, RoutedEventArgs e){
            if (Convert.ToBoolean(toTextBoxPoem.IsChecked)) writePoemToScreen = true;
            if (Convert.ToBoolean(toFilePoem.IsChecked)) writePoemToFile = true;
        }

        private void UnheckBox_DestinationPoem(object sender, RoutedEventArgs e) { 
            if (Convert.ToBoolean(!toTextBoxPoem.IsChecked)) writePoemToScreen = false;
            if (Convert.ToBoolean(!toFilePoem.IsChecked)) writePoemToFile = false;
        }

        private void Button_AdvencedSettings(object sender, RoutedEventArgs e)
        {
            var advancedSettings = new AdvancedSettings(sentenceRoot);
            advancedSettings.Show();
        }

        SentenceNode sentenceRoot;

        public void setSentenceRoot(SentenceNode s)
        {
            Debug.WriteLine("Ustawiono SentenceNode");
            sentenceRoot = s;
        }

        private void newSentenceRoot(object sender, RoutedEventArgs e)
        {
            var s = sentenceRoot.getSylabizator();
            sentenceRoot = new SentenceNode("NULL",s,this);
            loadedPoemFiles.Clear();
            updatePoemFileList();

            poemButton.IsEnabled = false;
            PoemReset.IsEnabled = false;
        }

        private void timerPoem_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        
        //////////////////////////////////////////////////


        void createNewWords(
            int amount = 10000, int level = 2, string fileName = "CreatedWords.txt")
        {
            Nodev2 root = new Nodev2(' ');
            root.createGraph(level);
            root.teach(level);

            string[] results = new string[amount];
            for (int i = 0; i < amount; i++)
                results[i] = root.generateNewWord(level);

            try
            {
                File.WriteAllLines(fileName, results);
            }
            catch (IOException e)
            {
                Console.Error.WriteLine("Error");
            }
        }

        /*void createPoem(
            int numberOfLine = 16, string poemFile = "Poem.txt", string wordsFile = "CreatedWords.txt")
        {
            Poem poem = new Poem(wordsFile);
            poem.createPoem(numberOfLine, poemFile);
        }

        static void createNewWordsPolish(
            int amount = 10000, int level = 2, string fileName = "CreatedWordsPolish.txt")
        {
            NodePl root = new NodePl(' ');
            root.createGraph(level);
            root.teach(level);

            string[] result = new string[amount];
            for (int i = 0; i < amount; i++)
                result[i] = root.generateNewWord(level);

            try
            {
                File.WriteAllLines(fileName, result);
            }
            catch (IOException e)
            {
                Console.Error.WriteLine("Error!");
            }
        }*/
    }
}
