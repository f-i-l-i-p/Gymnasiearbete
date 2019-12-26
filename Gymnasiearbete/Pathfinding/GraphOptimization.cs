using Gymnasiearbete.Graphs;
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
        public static void ShrinkGraph(Graph graph, IEnumerable<Node> avoid)
        {
            // For each node in the graph, check if it only has two neighbors.
            // If it has two neighbors (and is not in {avoid}), remove the node and connect the neighbors

            // Loop trough all nodes in graph
            for (int nodeId = 0; nodeId < graph.Nodes.Count; nodeId++)
            {
                var node = graph.Nodes[nodeId];
                if (node == null)
                    continue;

                // If node has two neighbors
                if (node.Adjacents.Count == 2 && !avoid.Contains(node))
                {
                    // the total weight between the neighbors
                    var weight = node.Adjacents[0].Weight + node.Adjacents[1].Weight;

                    // Remove edges from the neighbors to the node
                    foreach (var adjacent in node.Adjacents)
                    {
                        graph.Nodes[adjacent.Id].Adjacents.RemoveAll(x => x.Id == node.Id);
                    }

                    // remove node
                    graph.Nodes[node.Id] = null;

                    // connect neighbors
                    graph.AddEdge(graph.Nodes[node.Adjacents[0].Id], graph.Nodes[node.Adjacents[1].Id], weight);
                }
            }
        }
    }
}
