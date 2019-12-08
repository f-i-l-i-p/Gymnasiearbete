using System;
using System.Collections.Generic;
using System.Linq;

namespace Gymnasiearbete.MazeGeneration
{
    static class MazeGenerator
    {
        private static Random random = new Random();
        
        private static List<int> GetUnvissitedNeighbors(Graph maze, int sideLength, int node, bool includeVissited)
        {
            // node coordinates: { x, y }
            var nodePos = new int[] { node % sideLength, node / sideLength };

            // All possible neighbor positions relative to the node
            var relPos = new List<int[]>
                {
                    new int[] {  0, -1 },
                    new int[] {  1,  0 },
                    new int[] {  0,  1 },
                    new int[] { -1,  0 },
                };

            var neighbors = new List<int>();

            foreach (var p in relPos)
            {
                // calculate neighbor position
                var np = new int[] { p[0] + nodePos[0], p[1] + nodePos[1] };
                // neighbor id
                var id = np[0] + np[1] * sideLength;

                // Check if the position is inside the maze and if it is unvisited if onlyUnvissited=true
                if (np[0] >= 0 && np[0] < sideLength && np[1] >= 0 && np[1] < sideLength
                    // if unvissited (as not been conneted to another node)
                    && (includeVissited || !maze.AdjacencyList[id].Any()))
                {
                    // Save the unvisited neighbor
                    neighbors.Add(id);
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Generates a maze with {size} nodes.
        /// Each side will have the length of ToInt32(Sqrt(size)).
        /// If the maze size can't be evenly squared, the size will be rounded to Pow(ToInt32(Sqrt(size)), 2).
        /// </summary>
        /// <param name="size">The number of nodes in the maze.</param>
        /// <param name="openness">How open the maze is. When openness == 1, each node will be connected to all its neighbors.</param>
        /// <returns>The maze as a graph.</returns>
        public static Graph GenerateMaze(int size, double openness)
        {
            var graph = new Graph();

            int sideLength = Convert.ToInt32(Math.Sqrt(size));


            // Add all nodes to the graph:
            //
            //   0   1   2   3
            //
            //   4   5   6   7
            //
            //   8   9   10  11
            //
            //   12  13  14  15
            //
            for (int x = 0; x < sideLength; x++)
            {
                for (int y = 0; y < sideLength; y++)
                {
                    graph.AddNode(new Possition
                    {
                        X = x,
                        Y = y,
                    });
                }
            }


            var nodeStack = new Stack<int>();
            // add a start node
            nodeStack.Push(0);

            // Connect nodes:
            //
            //   0---1---2---3
            //       |       |
            //   4   5   6---7
            //   |   |       |
            //   8---9---10--11
            //           |   
            //   12--13--14--15
            //
            while (nodeStack.Any())
            {
                // get current node from stack
                var currentNode = nodeStack.Peek();

                // get all unvisited neighbors around the current cell
                var unbs = GetUnvissitedNeighbors(graph, sideLength, currentNode, false);

                // If there are any unvisited neighbors
                if (unbs.Any())
                {
                    // chose a random neighbor as the next node
                    var nextNode = unbs[random.Next(0, unbs.Count)];

                    // connect the nodes
                    graph.AddEdge(currentNode, nextNode);

                    // add the new node to the stack
                    nodeStack.Push(nextNode);
                }
                else
                {
                    // remove the current node from the stack
                    nodeStack.Pop();
                }
            }


            // If oppenes == 0, then no extra edges will be added
            if (openness <= 0)
                return graph;

            // Add extra edges:
            //
            //   0---1---2---3
            //       |   |   |
            //   4---5   6---7
            //   |   |       |
            //   8---9---10--11
            //       |   |   
            //   12--13--14--15
            //
            // Loop thorugh all nodes
            for (int node = 0; node < graph.AdjacencyList.Count; node++)
            {
                // get all neighbors around the node
                var nbs = GetUnvissitedNeighbors(graph, sideLength, node, true);

                // for each neighbor
                foreach (var nb in nbs)
                {
                    // If already connected to node
                    if (graph.AdjacencyList[node].Contains(nb))
                        continue;

                    // Probablity that it connects with the node: (openness)/1.
                    if (random.NextDouble() < openness)
                        // connect node and neighbor
                        graph.AddEdge(node, nb);
                }
            }

            return graph;
        }
    }
}
