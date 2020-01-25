using Gymnasiearbete.Graphs;
using System.Collections.Generic;
using System.Linq;

namespace Gymnasiearbete.Pathfinding
{
    static class GraphOptimization
    {
        public enum OptimizationType { None, IntersectionJumps, CornerJumps }

        /// <summary>
        /// Removes all nodes with two neighbors and instead connects the neighbors.
        /// The new cost to travel between the neighbors will be the same as the cost before.
        /// </summary>
        /// <param name="graph">Graph to add intersection jumps to.</param>
        /// <param name="avoid">Nodes that will not be removed.</param>
        public static void IntersectionJumps(Graph graph, IEnumerable<Node> avoid)
        {
            // For each node in the graph, check if it only has two neighbors.
            // If it has two neighbors (and is not in {avoid}), remove the node and connect the neighbors

            // Loop trough all nodes in graph
            for (int nodeId = 0; nodeId < graph.Nodes.Count; nodeId++)
            {
                var node = graph.Nodes[nodeId];
                if (node == null)
                    continue;

                if (node.Adjacents.Count == 2 && !avoid.Contains(node))
                    JumpOver(node, graph);
            }
        }

        /// <summary>
        /// Removes all nodes that are not corners and instead connects the neighbors
        /// (nodes that are connected to more than two nodes, or nodes that are dead-ends, is also counted as corners).
        /// The new cost to travel between the neighbors will be the same as the cost before.
        /// </summary>
        /// <param name="graph">Graph to add corner jumps to.</param>
        /// <param name="avoid">Nodes that will not be removed.</param>
        public static void CornerJumps(Graph graph, IEnumerable<Node> avoid)
        {
            // For each node in the graph, check if it is a corner.
            // If it is a corner (and is not in {avoid}), remove the node and connect the neighbors

            // Loop through all nodes in the graph
            foreach (var node in graph.Nodes)
            {
                if (node.Adjacents.Count == 2 && !avoid.Contains(node) && IsCorner(node, graph))
                    JumpOver(node, graph);
            }
        }

        /// <summary>
        /// Disconnects a node with its neighbors and instead connects the neighbors to each other.
        /// </summary>
        /// <param name="node">Node to disconnect.</param>
        /// <param name="graph">Graph containing the node.</param>
        private static void JumpOver(Node node, Graph graph)
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

        /// <summary>
        /// Checks if a node is a corner.
        /// </summary>
        /// <param name="graph">Graph containing the node.</param>
        /// <param name="node">Node to check.</param>
        /// <returns>True if the node is a corner, false if the node is not a corner.</returns>
        private static bool IsCorner(Node node, Graph graph)
        {
            // If more than 2 neighbors or dead-end
            if (node.Adjacents.Count > 2 || node.Adjacents.Count == 1)
                return true;

            else if (node.Adjacents.Count == 2)
            {
                // Neighbor positions
                var pos0 = graph.Nodes[node.Adjacents[0].Id].Position;
                var pos1 = graph.Nodes[node.Adjacents[1].Id].Position;

                // If vertical or horizontal neighbors
                if (pos0.X == pos1.X || pos0.Y == pos1.Y)
                    return false;
            }

            // no neighbors
            return false;
        }
    }
}
