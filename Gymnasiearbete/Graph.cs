using System.Collections.Generic;
using System.Linq;

namespace Gymnasiearbete
{
    class Graph
    {
        public List<List<int>> AdjacencyList { get; }

        public Graph(int nodes = 0)
        {
            AdjacencyList = Enumerable.Repeat(new List<int>(), nodes).ToList();
        }

        // Adds a new edge to the list
        public void AddEdge(int node1, int node2) //, int weight)
        {
            AdjacencyList[node1].Add(node2);
            AdjacencyList[node2].Add(node1);
        }

        // Removes an edge
        public void RemoveEdge(int node1, int node2)
        {
            AdjacencyList[node1].Remove(node2);
            AdjacencyList[node2].Remove(node1);
        }

        // Adds a new node and returns the index position
        public int AddNode()
        {
            var index = AdjacencyList.FindIndex(e => e == null);

            // If no empty list position exists
            if (index == -1)
            {
                AdjacencyList.Add(new List<int>());
                return AdjacencyList.Count - 1;
            }
            else
            {
                AdjacencyList[index] = new List<int>();
                return index;
            }
        }

        // Removes a node
        public void RemoveNode(int node)
        {
            AdjacencyList[node] = null;
            AdjacencyList.ForEach(n => n.Remove(node));
        }
    }
}

