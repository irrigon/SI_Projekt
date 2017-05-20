using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sylabizator
{
    [Serializable]
    public class NodeConnection
    {
        private Node nodeA;

        public Node NodeA
        {
            get { return nodeA; }
            set { nodeA = value; }
        }

        private List<String> words;

        public List<String> Words
        {
            get { return words; }
            set { words = value; }
        }

        private Node nodeB;

        public Node NodeB
        {
            get { return nodeB; }
            set { nodeB = value; }
        }

        public NodeConnection(Node nodeA, Node nodeB)
        {
            this.nodeA = nodeA;
            this.nodeB = nodeB;
            this.overallCount = 0;
            this.words = new List<String>();
        }

        private int overallCount = 0;

        public int OverallCount
        {
            get { return overallCount; }
            set { overallCount = value; }
        }
    }
}
