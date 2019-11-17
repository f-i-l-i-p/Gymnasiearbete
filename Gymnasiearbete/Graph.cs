using System.Collections.Generic;
using System.Linq;

namespace Gymnasiearbete
{
    class Possition
    {
        /// <summary>
        /// Horizontal coordinate
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// Vertical coordinate
        /// </summary>
        public int Y { get; set; }
    }

    class Graph
    {
        public List<List<int>> AdjacencyList { get; }
        public List<Possition> NodePossitions { get; }

        public Graph()
        {
            AdjacencyList = new List<List<int>>();
            NodePossitions = new List<Possition>();
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
        public int AddNode(Possition possition)
        {
            var index = AdjacencyList.FindIndex(e => e == null);

            // If no empty list position exists
            if (index == -1)
            {
                AdjacencyList.Add(new List<int>());
                NodePossitions.Add(possition);
                return AdjacencyList.Count - 1;
            }
            else
            {
                AdjacencyList[index] = new List<int>();
                NodePossitions[index] = possition;
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

