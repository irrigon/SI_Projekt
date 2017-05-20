
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
    }
}
