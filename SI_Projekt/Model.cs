using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sylabizator
{
    [Serializable]
    public enum SylabizatorLanguage
    {
        Polish = 0,
        English = 1
    }

    [Serializable]
    public class Model
    {
        private SylabizatorLanguage sylLang;

        public SylabizatorLanguage SylLang
        {
            get { return sylLang; }
            set { sylLang = value; }
        }

        private Dictionary<String, Node> syllables;

        public Dictionary<String, Node> Syllables
        {
            get { return syllables; }
            set { syllables = value; }
        }

        // węzeł początkowy, taki sam jak każdy inny
        private Node startNode = new Node("");

        public Node StartNode
        {
            get { return startNode; }
            set { startNode = value; }
        }

        // węzeł koncowy, taki sam jak każdy inny
        private Node endNode = new Node("");

        public Node EndNode
        {
            get { return endNode; }
            set { endNode = value; }
        }

        private int[, , ,] tab = new int[35, 35, 35, 35];

        public int[, , ,] Tab
        {
            get { return tab; }
            set { tab = value; }
        }        

        public Model()
        {
            clearModel();
            sylLang = SylabizatorLanguage.English;
        }

        public void clearModel()
        {
            for (int a = 0; a < 35; a++)
            {
                for (int b = 0; b < 35; b++)
                {
                    for (int c = 0; c < 35; c++)
                    {
                        for (int d = 0; d < 35; d++)
                        {
                            tab[a, b, c, d] = 0;
                        }
                    }
                }
            }

            StartNode = new Node("");
            EndNode = new Node("");
            syllables = new Dictionary<string, Node>();
        }

        public void updateLanguage(SylabizatorLanguage sl)
        {
            sylLang = sl;
        }
    }
}
