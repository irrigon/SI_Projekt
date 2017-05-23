using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sylabizator
{
    [Serializable]
    public class Node
    {
        private List<NodeConnection> next;

        public List<NodeConnection> Next
        {
            get { return next; }
            set { next = value; }
        }

        private string syllable;

        public string Syllable
        {
            get { return syllable; }
            set { syllable = value; }
        }

        private int lastNodeCount = 0;

        public int LastNodeCount
        {
            get { return lastNodeCount; }
            set { lastNodeCount = value; }
        }

        public Dictionary<Node, int> getNextNodes()
        {
            Dictionary<Node, int> nodes = new Dictionary<Node, int>();

            foreach (NodeConnection nc in Next)
            {
                nodes.Add(nc.NodeB, nc.OverallCount);
            }

            return nodes;
        }

        public Node(string syllable)
        {
            next = new List<NodeConnection>();

            this.syllable = syllable;
        }
    }
}
