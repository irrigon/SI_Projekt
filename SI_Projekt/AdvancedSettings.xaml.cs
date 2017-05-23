
using System;
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
using System.Windows.Shapes;

using SI_Projekt;

namespace Projekt_SI_GUI
{
    /// <summary>
    /// Interaction logic for AdvancedSettings.xaml
    /// </summary>
    public partial class AdvancedSettings : Window
    {
        public AdvancedSettings(SentenceNode sentenceRoot)
        {
            this.sentenceRoot = sentenceRoot;
            InitializeComponent();
            
            sleInPoem = sentenceRoot.getSyllablesInVerse();
            rhymLife = sentenceRoot.getRhymeLife();
            lenStability = sentenceRoot.getVerseThreshold();
            amountTests1 = sentenceRoot.getMiniRepeatMax();
            amountTests2 = sentenceRoot.getRepeatMax();
            amountTests3 = sentenceRoot.getBigRepeatMax();

            setAmounts();
        }

        private void Button_Click(object sender, RoutedEventArgs e){
            Button btn = sender as Button;
            if (btn.Name.ToString() == "OK")
            {
                sentenceRoot.setSyllablesInVerse((int)AmountSleInRow.Value);
                sentenceRoot.setRhymeLife((int)RythmLife.Value);
                sentenceRoot.setVerseThreshold((int)lenSability.Value);
                sentenceRoot.setMiniRepeatMax((int)AmountTests1.Value);
                sentenceRoot.setRepeatMax((int)AmountTests2.Value);
                sentenceRoot.setBigRepeatMax((int)AmountTests3.Value);
                this.Close();
            }
            else if (btn.Name.ToString() == "Anuluj")
            {
                this.Close();
            }
        }

        public SentenceNode sentenceRoot;

        public static int sleInPoem { get; set; }    // liczba sylab w wierszu
        public static int rhymLife { get; set; }     // życie rymu
        public static int lenStability { get; set; } // stabliność długości wiersza
        public static int amountTests1 { get; set; } // ilość prób1
        public static int amountTests2 { get; set; } // ilość prób2
        public static int amountTests3 { get; set; } // ilość prób3

        private void setAmounts()
        {
            AmountSleInRow.Text = sleInPoem.ToString();
            RythmLife.Text = rhymLife.ToString();
            lenSability.Text = lenStability.ToString();
            AmountTests1.Text = amountTests1.ToString();
            AmountTests2.Text = amountTests2.ToString();
            AmountTests3.Text = amountTests3.ToString();
        }

        private void getAmout_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txb = sender as TextBox;
            int temp;
            if(Int32.TryParse(txb.Text, out temp)){
                switch(txb.Name.ToString()){
                    case "AmountSleInRow":
                        sleInPoem = temp;
                        break;
                    case "RythmLife":
                        rhymLife = temp;
                        break;
                    case "lenSability":
                        lenStability = temp;
                        break;
                    case "AmountTests1":
                        amountTests1 = temp;
                        break;
                    case "AmountTests2":
                        amountTests2 = temp;
                        break;
                    case "AmountTests3":
                        amountTests3 = temp;
                        break;
                }
            }

        } 
    }
}
