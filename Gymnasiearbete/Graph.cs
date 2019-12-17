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
        public List<Possition> NodePossitions { get; }

        public Graph()
        {
            AdjacencyList = new List<List<Adjacent>>();
            NodePossitions = new List<Possition>();
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
        public int AddNode(Possition possition)
        {
            var index = AdjacencyList.FindIndex(e => e == null);

            // If no empty list position exists
            if (index == -1)
            {
                AdjacencyList.Add(new List<Adjacent>());
                NodePossitions.Add(possition);
                return AdjacencyList.Count - 1;
            }
            else
            {
                AdjacencyList[index] = new List<Adjacent>();
                NodePossitions[index] = possition;
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

        /// <summary>
        /// Removes all nodes with two neighbors and instead connects the neighbors.
        /// The new cost to travel been the neighbors will be the same as the cost before.
        /// </summary>
        /// <param name="graph">Graph to shrink.</param>
        /// <param name="avoid">Nodes that will not be removed.</param>
        public static void ShrinkGraph(Graph graph, List<int> avoid)
        {
            // For each node in the graph, check if it only has two neighbors.
            // If it has two neighbors (and is not in {avoid}), remove the node and connect the neighbors

            // Loop trough all nodes in graph
            for(int node = 0; node < graph.AdjacencyList.Count; node++)
            {
                // node neighbors list
                var adjacents = graph.AdjacencyList[node];

                // If node has two neighbors
                if (adjacents.Count == 2 && !avoid.Contains(node))
                {
                    // the total weight between the neighbors
                    var weight = adjacents[0].Weight + adjacents[1].Weight;

                    // Remove edges from the neighbors to the node
                    foreach (var adjacent in adjacents)
                    {
                        graph.AdjacencyList[adjacent.Id].RemoveAll(adj => adj.Id == node);
                    }

                    // remove node
                    graph.AdjacencyList[node] = null;

                    // connect neighbors
                    graph.AddEdge(adjacents[0].Id, adjacents[1].Id, weight);
                }
            }
        }
    }
}

