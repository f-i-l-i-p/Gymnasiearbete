using System.Collections.Generic;
using System.Linq;

namespace Gymnasiearbete.Pathfinding
{
    static class GraphOptimization
    {
        public enum OptimizationType { None, Shrinked }

        /// <summary>
        /// Removes all nodes with two neighbors and instead connects the neighbors.
        /// The new cost to travel been the neighbors will be the same as the cost before.
        /// </summary>
        /// <param name="graph">Graph to shrink.</param>
        /// <param name="avoid">Nodes that will not be removed.</param>
        public static void ShrinkGraph(Graph graph, IEnumerable<int> avoid)
        {
            // For each node in the graph, check if it only has two neighbors.
            // If it has two neighbors (and is not in {avoid}), remove the node and connect the neighbors

            // Loop trough all nodes in graph
            for (int node = 0; node < graph.AdjacencyList.Count; node++)
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
