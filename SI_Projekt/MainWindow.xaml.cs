
using System;
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
            poemButton.IsEnabled = true;
            poemButtonStop.IsEnabled = false;

            verseChecker.IsChecked = true;
            letterChecker.IsChecked = true;
            identChecker.IsChecked = true;
            rhymeChecker.IsChecked = true;

            //ContentTextBox.Text = "do tego pola przypisujcie wygenerowane słowa";
            //ContentTextBoxPoem.Text = "do tego pola przypisujcie wygenerowane wiersze";
        }

        public delegate void UpdateTextCallback(string message);
        public delegate void UpdateVoidCallback();

        public void updatePoem(string str)
        {
            ContentTextBoxPoem.Text = str;
        }

        private void generatePoem(object sender, RoutedEventArgs e)
        {
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

            EnglishPoem.IsEnabled = false;
            PolishPoem.IsEnabled = false;

            toFilePoem.IsEnabled = false;
            toTextBoxPoem.IsEnabled = false;
            verseChecker.IsEnabled = false;
            letterChecker.IsEnabled = false;
            identChecker.IsEnabled = false;
            rhymeChecker.IsEnabled = false;

            Console.WriteLine((bool)rhymeChecker.IsChecked);
            sentenceRoot.setChecks( (bool)verseChecker.IsChecked,
                                    (bool)letterChecker.IsChecked,
                                    (bool)rhymeChecker.IsChecked,
                                    (bool)identChecker.IsChecked );

            int x;
            Int32.TryParse(VerseAmout.Text, out x);
            Task.Run(() => sentenceRoot.generatePoem(x, polish));

            //sentenceRoot.generatePoem(8, 7, 2);
            //await Task.Factory.StartNew(() => sentenceRoot.generatePoem(8, 7, 2));
        }

        private void stopPoem(object sender, RoutedEventArgs e)
        {
            unlockEverything();
            sentenceRoot.stop();
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

            EnglishPoem.IsEnabled = true;
            PolishPoem.IsEnabled = true;

            toFilePoem.IsEnabled = true;
            toTextBoxPoem.IsEnabled = true;
            verseChecker.IsEnabled = true;
            letterChecker.IsEnabled = true;
            identChecker.IsEnabled = true;
            rhymeChecker.IsEnabled = true;
        }

        private void Button_Chose(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var dlg = new System.Windows.Forms.FolderBrowserDialog() { Description = "Select directory to open" };

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK){
                if (btn.Name.ToString() == "src") {
                    sSelectedPath = dlg.SelectedPath;
                    sFilePath.Text = sSelectedPath;
                }
                else if (btn.Name.ToString() == "dest") {
                    dSelectedPath = dlg.SelectedPath;
                    dFilePath.Text = dSelectedPath;
                }
            }
        }

        private void RadioButton_Language(object sender, RoutedEventArgs e){
            RadioButton rdo = sender as RadioButton;
            if (rdo.Name.ToString() == "Polish") {
                polish = true;
            }
            else if (rdo.Name.ToString() == "English"){
                polish = false;
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
        private string dPoemSelectedPath;
        private string dPoemSelectedPath2;
        private bool writePoemToFile = false;
        private bool writePoemToScreen = false;
 

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

        private void Button_Chose_Poem(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var dlg = new System.Windows.Forms.FolderBrowserDialog() { Description = "Select directory to open" };

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (btn.Name.ToString() == "srcPoem")
                {
                    sPoemSelectedPath = dlg.SelectedPath;
                    sPoemFilePath.Text = dlg.SelectedPath;
                }
                else if (btn.Name.ToString() == "destPoem")
                {
                    dPoemSelectedPath = dlg.SelectedPath;
                    dPoemFilePath.Text = dlg.SelectedPath;
                }
                /*else if (btn.Name.ToString() == "destPoem2") {
                    dPoemSelectedPath2 = dlg.SelectedPath;
                    dPoemFilePath2.Text = dlg.SelectedPath;
                }*/
            }
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
    }
}
