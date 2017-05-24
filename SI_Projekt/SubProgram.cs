using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Projekt_SI_GUI;

namespace SI_Projekt
{
    class SubProgram
    {
        //////////////////////////////////////////////////

        public SubProgram(MainWindow window)
        {
            this.window = window;
        }

        MainWindow window;
        public bool stopped = false;

        public void createNewWords(
            int amount = 10000, int level = 2, string input="NULL", string fileName = "CreatedWords.txt")
        {
            Nodev2 root = new Nodev2(' ');
            root.createGraph(level);
            root.teach(level, input);

            stopped = false;
            clearWindowText();

            string[] results = new string[amount];
            for (int i = 0; i < amount; i++)
            {
                results[i] = root.generateNewWord(level);
                addToWindowText(results[i]);
                if (stopped) { return; }
            }

            unlockWindow();

            if ((fileName != "") && (fileName != null))
            {
                try
                { File.WriteAllLines(fileName, results); }
                catch (IOException e)
                { Console.Error.WriteLine("Error"); }
            }
        }

        void createPoem(
            int numberOfLine = 16, string poemFile = "Poem.txt", string wordsFile = "CreatedWords.txt")
        {
            Poem poem = new Poem(wordsFile);
            poem.createPoem(numberOfLine, poemFile);
        }

        public void createNewWordsPolish(
            int amount = 10000, int level = 2, string input = "NULL", string fileName = "CreatedWordsPolish.txt")
        {
            NodePl root = new NodePl(' ');
            root.createGraph(level);
            root.teach(level, input);

            stopped = false;
            clearWindowText();

            string[] result = new string[amount];
            for (int i = 0; i < amount; i++)
            {
                result[i] = root.generateNewWord(level);
                addToWindowText(result[i]);
                if (stopped) { return; }
            }

            unlockWindow();

            if ((fileName != "") && (fileName != null))
            {
                try
                { File.WriteAllLines(fileName, result); }
                catch (IOException e)
                { Console.Error.WriteLine("Error"); }
            }
        }

        protected void clearWindowText()
        {
            window.ContentTextBox.Dispatcher.Invoke(
                new MainWindow.UpdateVoidCallback(window.clearWords),
                    new object[] { });
        }

        protected void addToWindowText(string tmp)
        {
            window.ContentTextBox.Dispatcher.Invoke(
                new MainWindow.UpdateTextCallback(window.addToWords),
                    new object[] { tmp + "\n" });
        }

        protected void unlockWindow()
        {
            window.ContentTextBox.Dispatcher.Invoke(
                new MainWindow.UpdateVoidCallback(window.unlockEverythingInWords),
                    new object[] { });
        }
    }
}
