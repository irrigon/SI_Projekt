using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Sylabizator
{
    public class Sylabizator
    {        
        private Model model;
        public List<String> addedWords;


        public Model Model
        {
            get { return model; }
            set { model = value; }
        }

        public Sylabizator()
        {
            model = new Model();
        }

        #region learn by words        

        public void learnByWords(String text)
        {
            List<string> words = divideIntoWords(text.ToLower(),true);
            int[] t = new int[4];

            for (int i = 0; i < words.Count; i++)
            {
                t = getCharToIntArray(words[i]);

                model.Tab[t[0], t[1], t[2], t[3]] += 1;
                if (t[3] != 34)
                {
                    model.Tab[t[0], t[1], t[2], 34] += 1;    
                }
                if (t[2] != 34)
                {
                    model.Tab[t[0], t[1], 34, 34] += 1;
                }
                if (t[1] != 34)
                {
                    model.Tab[t[0], 34, 34, 34] += 1;
                }
            }
        }

        private int[] getCharToIntArray(string str)
        {
            int[] t = new int[4];

            t[0] = 34;
            if (str.Length >= 1)
            {
                t[0] = charToInt(str[0]);
            }
            t[1] = 34;
            if (str.Length >= 2)
            {
                t[1] = charToInt(str[1]);
            }
            t[2] = 34;
            if (str.Length >= 3)
            {
                t[2] = charToInt(str[2]);
            }
            t[3] = 34;
            if (str.Length >= 4)
            {
                t[3] = charToInt(str[3]);
            }

            return t;
        }

        private int charToInt(Char c)
        {
            switch (c)
            {
                case 'a': return 0;
                case 'ą': return 1;
                case 'b': return 2;
                case 'c': return 3;
                case 'ć': return 4;
                case 'd': return 5;
                case 'e': return 6;
                case 'ę': return 7;
                case 'f': return 8;
                case 'g': return 9;
                case 'h': return 10;
                case 'i': return 11;
                case 'j': return 12;
                case 'k': return 13;
                case 'l': return 14;
                case 'ł': return 15;
                case 'm': return 16;
                case 'n': return 17;
                case 'ń': return 18;
                case 'o': return 19;
                case 'ó': return 20;
                case 'p': return 21;
                case 'r': return 22;
                case 's': return 23;
                case 'ś': return 24;
                case 't': return 25;
                case 'u': return 26;
                case 'v': return 27;
                case 'w': return 28;
                case 'x': return 29;
                case 'y': return 30;
                case 'z': return 31;
                case 'ź': return 32;
                case 'ż': return 33;
                default: return 34;
            }
        }

        private int compareSyllableBeginnings(string a, string b)
        {
            int[] ta = new int[4];
            int[] tb = new int[4];

            ta = getCharToIntArray(a);
            tb = getCharToIntArray(b);

            //int[, , ,] x = new int[ta[0], ta[1], ta[2], ta[3]];
            //int[, , ,] y = new int[tb[0], tb[1], tb[2], tb[3]];

             
            int result = model.Tab[ta[0], ta[1], ta[2], ta[3]].CompareTo(model.Tab[tb[0], tb[1], tb[2], tb[3]]);

            if (result == 0)
            {
                result = model.Tab[ta[0], ta[1], ta[2], 34].CompareTo(model.Tab[tb[0], tb[1], tb[2], 34]);
            }
            if (result == 0)
            {
                result = model.Tab[ta[0], ta[1], 34, 34].CompareTo(model.Tab[tb[0], tb[1], 34, 34]);
            }
            if (result == 0)
            {
                result = model.Tab[ta[0], 34, 34, 34].CompareTo(model.Tab[tb[0], 34, 34, 34]);
            }

            return result;
        }

        #endregion

        #region learn by syllables        

        public void learnBySyllables(String text, bool uniq)
        {
            List<string> words = divideIntoWords(text.ToLower(),false);
            String wordsList = "";
            addedWords = new List<String>();

            for (int i = 0; i < words.Count; i++)
            {
                String[] syl = words[i].Split('-');
                String word = "";
                String wholeWord = "";

                for (int k = 0; k < syl.Length; k++)
                {
                    wholeWord += syl[k];
                }

                wholeWord = wholeWord.Trim();

                if (uniq == true && (addedWords.Contains(wholeWord) || wholeWord.CompareTo("") == 0))
                    continue;

                for (int k = 0; k < syl.Length - 1; k++)
                {
                    if (k == 0)
                    {
                        word += syl[k] + syl[k + 1];
                    }
                    else
                    {
                        word += syl[k + 1];
                    }

                    // insert connection beetween syl[k] & syl[k+1]
                    String syllableA = syl[k];
                    String syllableB = syl[k + 1];

                    Node a = null;
                    if (model.Syllables.ContainsKey(syllableA))
                    {
                        a = model.Syllables[syllableA];
                    }
                    else
                    {
                        a = new Node(syllableA);
                        model.Syllables.Add(syllableA, a);                        
                    }

                    if (k == 0) // dodajemy startowy wezel
                    {
                        NodeConnection w = null;
                        foreach (NodeConnection nc in model.StartNode.Next)
                        {
                            if (nc.NodeB.Syllable == syllableA)
                            {
                                w = nc;
                                break;
                            }
                        }
                        if (w == null)
                        {
                            w = new NodeConnection(model.StartNode, a);

                            model.StartNode.Next.Add(w);
                        }

                        w.OverallCount++;
                        w.Words.Add(wholeWord);
                    }

                    Node b = null;
                    if (model.Syllables.ContainsKey(syllableB))
                    {
                        b = model.Syllables[syllableB];
                    }
                    else
                    {
                        b = new Node(syllableB);
                        model.Syllables.Add(syllableB, b);
                    }

                    if (k == syl.Length - 2) // dodajemy wezel konca
                    {
                        b.LastNodeCount++;
                    }

                    NodeConnection u = null;
                    foreach (NodeConnection nc in a.Next)
                    {

                        if (nc.NodeB.Syllable == b.Syllable)
                        {
                            u = nc;
                            break;
                        }
                    }
                    if (u == null)
                    {
                        u = new NodeConnection(a, b);

                        a.Next.Add(u);
                    }

                    u.OverallCount++;
                    u.Words.Add(wholeWord);
                }                

                addedWords.Add(word);

                wordsList += word + " ";
            }

            learnByWords(wordsList);
        }

        private int getNodeConnectionOverallCount(string syllableA, string syllableB)
        {
            int cnt = 0;

            if (model.Syllables.ContainsKey(syllableA))
            {
                Node a = model.Syllables[syllableA];
                foreach (NodeConnection nc in a.Next)
                {
                    if (syllableB.StartsWith(nc.NodeB.Syllable))
                    {
                        cnt += nc.OverallCount;
                    }
                }
            }

            return cnt;
        }


        #endregion

        #region divide into syllables

        private bool isVowel(Char c)
        {
            return isVowel(c.ToString());
        }

        private bool isOneLetterVowel(string s)
        {
            if (model.SylLang == SylabizatorLanguage.Polish)
            {
                if (s == "a" || s == "e" || s == "i" || s == "o" || s == "u" || s == "y" || s == "ą" || s == "ę" || s == "ó" )
                {
                    return true;
                }
            }
            else
            {
                // 'y' and 'w' sometimes act as vowels but without knowing how to spell word we cannot decide
                if (s == "a" || s == "e" || s == "i" || s == "o" || s == "u")
                {
                    return true;
                }
            }
            return false;
        }

        private bool isVowel(String s)
        {
            if (isOneLetterVowel(s))
            {
                return true;
            }
            else if(s.Length == 2) {
                if(s == "ow" || s == "ou" || s == "ie" || s == "oi" || s == "oy" || s == "ee" || s == "ai" || s == "ur" || s == "au" || s == "eu") { // ALUSIA 7.01 - mac => dodanie au i eu
                   return true;
                }
            }           
            return false;
        }

        private List<int> getVowelsIndexes(List<String> list)
        {
            List<int> ind = new List<int>();

            for (int i = 0 ; i < list.Count; i++)
            {
                // second part for dyphtongs (au, eu)
                // if (isVowel(list[i]) && /*!*/(list.Count > 1 /*&& i==0 */ && (list[/*0*/i]=="a" || list[/*0*/i]=="e") && list[/*1*/i+1]=="u")) // ALUSIA 7.01 - mac -> nie rozumiem czemy dyftongi są w tym miejscu, po czo :P
                if (isVowel(list[i]) && !(list.Count > 1 && i==0 && (list[0]=="a" || list[0]=="e") && list[1]=="u")) 
                {
                    ind.Add(i);
                }
            }

            return ind;
        }

        private bool isDoubleLetterSound(String str)
        {
            if (model.SylLang == SylabizatorLanguage.Polish)
            {
                switch (str)
                {
                    case "sz":
                        return true;
                    case "cz":
                        return true;
                    case "dz":
                        return true;
                    case "dź":
                        return true;
                    case "dż":
                        return true;
                    case "rz":
                        return true;
                    case "ch":
                        return true;
                    default:
                        return false;
                }
            }
            else if(model.SylLang == SylabizatorLanguage.English)
            {
                switch (str)
                {
                    case "th":
                        return true;
                    case "sh":
                        return true;
                    case "ph":
                        return true;
                    case "ch":
                        return true;
                    case "wh":
                        return true;
                    case "gh":
                        return true;
                    // diphtongs
                    case "ow":
                        return true;
                    case "ou":
                        return true;
                    case "ie":
                        return true;
                    case "oi":
                        return true;
                    case "ee":
                        return true;
                    case "ai":
                        return true;
                    case "ur":
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }

        private bool isSoundLessI(String input, int index)
        {
            if (model.SylLang == SylabizatorLanguage.Polish && input.Length - 1 >= index + 2)
            {
                //we can check SoundLess I
                if(input[index+1] == 'i' && isVowel(input[index+2]))
                {
                    return true;
                }
            }
            return false;
        }

        private List<String> divideIntoSounds(String input)
        {
            List<String> result = new List<string>();

            for (int index = 0; index < input.Length; index++)
            {
                if (index == input.Length - 1)
                {
                    if (model.SylLang == SylabizatorLanguage.English && input.Substring(index, 1) == "e")
                    {
                        result[result.Count - 1] += "e";                           
                    }
                    else
                    {
                        // cannot check doubleLettersSounds as last sound
                        result.Add(input.Substring(index, 1));
                    }
                } 
                else 
                {
                    String str = input.Substring(index, 2);

                    if(str.Length > 1 && (str[0]=='a' || str[0]=='e') && str[1] == 'u' ) // ALUSIA 7.01 - mac => cały if (ale to w elsie już nie)
                    {
                        //str += "u";
                        index++;
                    }
                    else
                    if (isDoubleLetterSound(str))
                    {
                        if (isSoundLessI(input, index + 1))
                        {
                            str += "i";
                            index++;
                        }                        
                        index++;
                    }
                    else
                    {
                        str = input[index].ToString();
                        if (isSoundLessI(input, index))
                        {
                            str += "i";
                            index++;
                        }    
                    }

                    result.Add(str);
                }
            }

            return result;            
        }

        public List<string> divideIntoWords(string input, bool replaceDash)
        {
            String text;
            
            if (replaceDash)
            {
                text = Regex.Replace(input, "[^a-ząćęłńśóźż]", " ");
            }
            else
            {
                text = Regex.Replace(input, "[^a-ząćęłńśóźż-]", " ");
            }
            text = Regex.Replace(text, "[ ]{2,}", " ");

            return text.Split(' ').ToList();
        }        

        public List<string> divideIntoSyllables(List<string> words, bool uniq)
        {
            List<String> result = new List<string>();
            addedWords = new List<string>();

            if (uniq)
            {
                addedWords = words.Distinct().ToList();
            }
            else
            {
                addedWords = words.ToList();
            }

            for (int i = 0; i < addedWords.Count; i++)
            {
                List<String> sList = divideIntoSounds(words[i]);
                List<int> vList = getVowelsIndexes(sList);
                bool isDoubleConsonant = false;
                int indexDoubleConsonant = 0;
                String output = "";
                int sInd = 0;
                int eInd = 0;
                if (vList.Count == 0) output = words[i]; // ALUSIA        

                for (int k = 0; k < vList.Count; k++)
                {
                    if (k == 0)
                    {
                        // first syllable
                        sInd = 0;
                    }
                    else
                    {
                        if (vList[k] - vList[k - 1] == 1)
                        {
                            // two vowels next to each other
                            sInd = vList[k];
                        }
                        else
                        {
                            // something in between
                            if (isDoubleConsonant)
                            {
                                sInd = indexDoubleConsonant;
                            }
                            else
                            {
                                //sInd = vList[k] - 1;
                                sInd = eInd + 1;
                            }
                        }
                    }

                    if (k == vList.Count - 1)
                    {
                        // last syllable
                        eInd = sList.Count - 1;
                    }
                    else
                    {
                        if (vList[k + 1] - vList[k] == 1)
                        {
                            // two vowels next to each other
                            eInd = vList[k];
                        }
                        else
                        {
                            // something in between

                            // check DoubleConsonant
                            isDoubleConsonant = false;

                            for (int p = vList[k] + 2; p < vList[k + 1]; p++)
                            {
                                if (sList[p][0] == sList[p - 1][sList[p-1].Length-1])
                                {
                                    isDoubleConsonant = true;
                                    indexDoubleConsonant = p;
                                    p = vList[k + 1];
                                }
                            }

                            if (isDoubleConsonant)
                            {
                                eInd = indexDoubleConsonant - 1;
                            }
                            else
                            {
                                //eInd = vList[k + 1] - 2;                                

                                int bestEIndBySyllables = 0;
                                int bestCnt = 0;

                                int f = 0; // ALUSIA 7.01 - mac
                                if (vList[k] + 2 != vList[k + 1]) f++; // ALUSIA 7.01 - mac
                                // i potem w forze było normalnie int f = vList[k]+1


                                for (f += vList[k]; f <= vList[k + 1] - 2; f++)
                                {
                                    //create beginning of possible new syllable
                                    StringBuilder sb = new StringBuilder();
                                    for (int h = f + 1; h < sList.Count; h++)
                                    {
                                        sb.Append(sList[h]);
                                    }

                                    string strB = sb.ToString();

                                    //create first syllable
                                    sb.Remove(0, sb.Length);
                                    for (int h = sInd; h <= f; h++)
                                    {
                                        sb.Append(sList[h]);
                                    }

                                    string strA = sb.ToString();

                                    int cnt = getNodeConnectionOverallCount(strA, strB);

                                    // can change > to >= to make second syllable longer
                                    if (cnt > bestCnt)
                                    {
                                        bestEIndBySyllables = f;
                                        bestCnt = cnt;
                                    }
                                }

                                if (bestCnt == 0)
                                {
                                    int bestEIndByWords = 0;
                                    string bestBeginning = "";
                                    bool isFirst = true;

                                    f = 0; // ALUSIA 7.01 - mac
                                    if (vList[k] + 2 != vList[k + 1]) f++; // ALUSIA 7.01 - mac
                                    // i potem w forze było normalnie int f = vList[k]+1

                                    for (f += vList[k]; f <= vList[k + 1] - 2; f++)
                                    {
                                        //create beginning of possible new syllable
                                        StringBuilder sb = new StringBuilder();
                                        for (int h = f + 1; h < sList.Count; h++)
                                        {
                                            sb.Append(sList[h]);
                                        }
                                        string str = sb.ToString();

                                        if (isFirst)
                                        {
                                            bestEIndByWords = f;
                                            bestBeginning = str;
                                            isFirst = false;
                                        }
                                        else
                                        {
                                            // can change >0 to >=0 to make second syllable longer
                                            if (compareSyllableBeginnings(str, bestBeginning) > 0)
                                            {
                                                bestEIndByWords = f;
                                                bestBeginning = str;
                                            }
                                        }
                                    }

                                    eInd = bestEIndByWords;
                                }
                                else
                                {
                                    eInd = bestEIndBySyllables;
                                }
                            }
                        }
                    }

                    // generate syllable
                    for (int f = sInd; f <= eInd; f++)
                    {
                        output += sList[f];
                    }
                    if (k < vList.Count - 1)
                    {
                        output += "-";
                    }
                }

                result.Add(output);
            }

            return result;
        }

        #endregion
    }
}
