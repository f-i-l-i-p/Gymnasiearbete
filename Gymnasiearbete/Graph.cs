using System.Collections.Generic;

namespace Gymnasiearbete
{
    class Position
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

    class Adjacent
    {
        /// <summary>
        /// Id of the adjacent node
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Cost to travel to the adjacent node
        /// </summary>
        public float Weight { get; set; }
    }

    class Graph
    {
        public List<List<Adjacent>> AdjacencyList { get; }
        public List<Position> NodePositions { get; }

        public Graph()
        {
            AdjacencyList = new List<List<Adjacent>>();
            NodePositions = new List<Position>();
        }

        // Adds a new edge to the list
        public void AddEdge(int node1, int node2, float weight = 1)
        {
            AdjacencyList[node1].Add(new Adjacent { Id = node2, Weight = weight,});
            AdjacencyList[node2].Add(new Adjacent { Id = node1, Weight = weight,});
        }

        // Removes an edge
        public void RemoveEdge(int node1, int node2)
        {
            AdjacencyList[node1].RemoveAll(adj => adj.Id == node2);
            AdjacencyList[node2].RemoveAll(adj => adj.Id == node1);
        }

        // Adds a new node and returns the index position
        public int AddNode(Position position)
        {
            var index = AdjacencyList.FindIndex(e => e == null);

            // If no empty list position exists
            if (index == -1)
            {
                AdjacencyList.Add(new List<Adjacent>());
                NodePositions.Add(position);
                return AdjacencyList.Count - 1;
            }
            else
            {
                AdjacencyList[index] = new List<Adjacent>();
                NodePositions[index] = position;
                return index;
            }
        }

        /// <summary>
        /// Removes a node and all edges to it.
        /// </summary>
        /// <param name="node">Node to remove.</param>
        public void RemoveNode(int node)
        {
            AdjacencyList[node] = null;
            AdjacencyList.ForEach(n => n.RemoveAll(adj => adj.Id == node));
        }
    }
}

