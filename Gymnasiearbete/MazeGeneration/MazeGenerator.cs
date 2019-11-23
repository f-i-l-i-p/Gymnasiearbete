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

        public static Graph GenerateMaze(int size, float openness)
        {
            var graph = new Graph();

            int sideLength = (int)Math.Sqrt(size);


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
                    // If the current node is only connected to 1 other node (it is a dead-end),
                    // then there will be a probablity that it connects to one more node.
                    // The probability for that is (openness)/1.
                    //
                    // If the current node is only connected to 1 other node:
                    if (graph.AdjacencyList[currentNode].Count == 1 &&
                        // Probability for conecting:
                        random.NextDouble() < openness)
                    {
                        // get neighbours
                        var nbs = GetUnvissitedNeighbors(graph, sideLength, currentNode, true);
                        // remove the node that the current node is already connected to
                        nbs.Remove(graph.AdjacencyList[currentNode][0]);
                        // chose node
                        var nodeToConnect = nbs[random.Next(0, nbs.Count)];
                        // connect the nodes
                        graph.AddEdge(currentNode, nodeToConnect);
                    }

                    // remove the current node from the stack
                    nodeStack.Pop();
                }
            }

            return graph;
        }
    }
}
