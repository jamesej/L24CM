using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace L24CM.Utility
{
    public class ArrayGraph<TNode, TEdge> : IComparer<TNode>
    {
        List<TNode> nodes = null;
        Dictionary<TNode, int> nodeIndex = null;
        TEdge[,] edges = null;
        int count;

        public bool Unidirectional { get; set; }

        public ArrayGraph(IEnumerable<TNode> nodes)
        {
            this.nodes = nodes.ToList();
            this.count = this.nodes.Count;
            this.edges = new TEdge[count, count];
            this.nodeIndex = new Dictionary<TNode, int>();
            for (int i = 0; i < this.count; i++)
                nodeIndex[this.nodes[i]] = i;

            Unidirectional = false;
        }

        public TEdge this[TNode from, TNode to]
        {
            get { return this.edges[nodeIndex[from], nodeIndex[to]]; }
            set
            {
                if (Unidirectional && !this[to, from].Equals(default(TEdge)))
                    throw new ArgumentException(
                        string.Format("Graph: cannot add edge from '{0}' to '{1}' as one exists in opposite direction",
                            from, to));
                this.edges[nodeIndex[from], nodeIndex[to]] = value;
            }
        }

        #region IComparer<TNode> Members

        /// <summary>
        /// This is a partial comparison, the return value is zero if no edge exists between the two nodes
        /// </summary>
        /// <param name="x">a node</param>
        /// <param name="y">another node</param>
        /// <returns>-1 if edge goes from x to y, 1 if edge goes from y to x, 0 if no edge between x and y</returns>
        public int Compare(TNode x, TNode y)
        {
            if (!this.edges[nodeIndex[x], nodeIndex[y]].Equals(default(TEdge)))
                return -1;
            else if (!this.edges[nodeIndex[y], nodeIndex[x]].Equals(default(TEdge)))
                return 1;
            else
                return 0;
        }

        #endregion
    }
}
