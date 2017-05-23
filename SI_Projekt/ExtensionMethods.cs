
using System;
using System.IO;
using System.Collections.Generic;

namespace SI_Projekt {
    public static class Extensions {

        public static string RemoveAll(this string s, string[] chars) {
            // Usuwanie wszelkich wystąpień
            // któregokolwiek z podanych znaków.
            // Jan Grzywacz.
            for (int i = 0; i < chars.Length; i++) {
                s = s.Replace(chars[i],String.Empty);
            }

            return s;
        }

        public static string Capitalize(this string s) {
            // Zmiana pierwszej litery na dużą.
            // Jan Grzywacz.
            string first = s[0].ToString().ToUpper();
            string rest = s.Substring(1);

            return first+rest;
        }

        public static string[] Syllables(this string s, Sylabizator.Sylabizator sylabizator) {
            // Zwracana listę sylab słowa.
            // Jan Grzywacz.
            List<string> wrapper = new List<string> { s };
            List<string> tmp = sylabizator.divideIntoSyllables(wrapper, false);
            string[] syllables = tmp[0].Split(new char[] { '-' });

            /*Console.WriteLine("");
            for (int i = 0; i < syllables.Length; i++) Console.WriteLine(syllables[i]);*/

            return syllables;
        }

        public static int LastVowels(this string s) {
            // Zwraca indeks ostatniej zbitki samogłosek,
            // nie licząc jednej, która ewentualnie znajduje się
            // na samym. końcu słowa.
            // Jan Grzywacz.

            int result = 0;
            bool found = false;
            for (int i = Math.Max(0,s.Length-2); i >= 0; i--) {
                if ("aeiouąęóy".IndexOf(s[i].ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0)
                    found = true;
                else if (found) {
                    result = i + 1;
                    break;
                }
            }

            //if ((result == s.Length - 1) && (result > 0)) result--;
            return Math.Min(Math.Max(0,result),s.Length-3);
        }

        public static int LastVowelsWeak(this string s) {
            // Słabsza wersja, dla polskich słów.
            // Jan Grzywacz.

            int result = 0;
            bool found = false;
            for (int i = Math.Max(0,s.Length-1); i >= 0; i--) {
                if ("aeiouąęóy".IndexOf(s[i].ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0)
                    found = true;
                else if (found) {
                    result = i + 1;
                    break;
                }
            }

            //if ((result == s.Length - 1) && (result > 0)) result--;
            return Math.Min(Math.Max(0,result),s.Length-3);
        }

        public static int LastVowelsOLD(this string s) {
            // Zwraca indeks ostatniej zbitki samogłosek.
            // Jan Grzywacz.

            // OSTATNIA LITERA JAKO SAMOGŁOSKA SIĘ NIE LICZY
            // JEDNA SPÓŁGŁOSKA PRZED, CHYBA ŻE OSTATNIA TO SAMOGŁOSKA (ALBO SMITH-WITH FAST-LAST JAPANESE-CHEESE)

            int result = 0;
            bool found = false;
            for (int i = s.Length - 1; i >= 0; i--) {
                if ("aeiouąęóy".IndexOf(s[i].ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0)
                    found = true;
                else if (found) {
                    result = i + 1;
                    break;
                }
            }

            //if ((result == s.Length - 1) && (result > 0)) result--;
            return Math.Min(Math.Max(0,result),s.Length-3);
        }

        public static string LastN(this string s, int n) {
            // Zwraca podciąg ostatnich n znaków.
            // Jan Grzywacz.
            return s.Substring(Math.Max(0,(s.Length-n)));
        }
    }
}
