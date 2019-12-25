using System;
using System.Collections.Generic;
using System.Linq;

namespace Gymnasiearbete.MazeGeneration
{
    static class MazeGenerator
    {
        private static readonly Random random = new Random();

        /// <summary>
        /// Generates a maze with {side*side} nodes with a set complexity.
        /// The complexity is a value between 0 and 1 and is an estimate how hard it will be to sole the maze.
        /// </summary>
        /// <param name="side">The length of each side in the maze.</param>
        /// <param name="complexity">Target complexity. Value between: 0 and 1.</param>
        /// <returns>The maze as a graph.</returns>
        public static Graph GenerateMaze(int side, double complexity)
        {
            var graph = GeneratePerfectMaze(side);

            // If complexity == 1, then no extra edges will be added
            if (complexity >= 1)
                return graph;

            ReduceComplexity(graph, complexity);


            // TODO: Remove
            foreach (var _node in graph.AdjacencyList)
            {
                if (_node == null)
                    throw new Exception();
            }

            return graph;
        }

        /// <summary>
        /// Reduces the complexity of a maze by adding random edges between nodes.
        /// If complexity == 0, then all nodes will be conneted.
        /// If complexity == 1, then no extra nodes will be connected.
        /// </summary>
        /// <param name="maze">Maze to reduce complexity in.</param>
        /// <param name="complexity">Target complexity. Value between: 0 and 1.</param>
        private static void ReduceComplexity(Graph maze, double complexity)
        {
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
            for (int node = 0; node < maze.AdjacencyList.Count; node++)
            {
                // get all neighbors around the node
                var nbs = GetNeighbors(maze, node);

                // for each neighbor
                foreach (var nb in nbs)
                {
                    // If already connected to node
                    if (maze.AdjacencyList[node].Exists(adj => adj.Id == nb))
                        continue;

                    // Probablity that it connects with the node: (1 - complexity).
                    if (random.NextDouble() >= complexity)
                        // connect node and neighbor
                        maze.AddEdge(node, nb);
                }
            }
        }

        /// <summary>
        /// Uses a recursive backtracker to generate a perfect maze
        /// as a graph with {side*side} nodes.
        /// </summary>
        /// <param name="side">The length of each side in the maze.</param>
        /// <returns>The maze as a Graph object.</returns>
        private static Graph GeneratePerfectMaze(int side)
        {
            var graph = new Graph();

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
            for (int x = 0; x < side; x++)
            {
                for (int y = 0; y < side; y++)
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

            // Connect nodes with recursive backtracker:
            //
            //   0---1---2---3
            //       |        
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
                var unbs = GetUnconecctedNeigbors(graph, currentNode);

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

            return graph;
        }

        /// <summary>
        /// Retruns a list of all nodes that are neighbors to the specified node
        /// (beside the specifed node if the graph is an even square) in the given graph.
        /// </summary>
        /// <param name="maze">Graph with all the nodes.</param>
        /// <param name="node">Node to find neighbors for.</param>
        /// <returns>A list with all neighbors.</returns>
        private static List<int> GetNeighbors(Graph maze, int node)
        {
            int side = (int)Math.Sqrt(maze.AdjacencyList.Count);

            // calculate node coordinates: { x, y }
            var nodePos = new int[] { node % side, node / side };

            // all possible neighbor positions relative to the node
            var relPos = new List<int[]>
            {
                new int[] {  0, -1 },
                new int[] {  1,  0 },
                new int[] {  0,  1 },
                new int[] { -1,  0 },
            };

            var neighbors = new List<int>();

            // for each relative neighbor possition
            foreach (var p in relPos)
            {
                // calculate neighbor position: { x, y }
                var np = new int[] { p[0] + nodePos[0], p[1] + nodePos[1] };

                // If neighbor is outside graph
                if (np[0] < 0 || np[0] >= side || np[1] < 0 || np[1] >= side)
                    continue;

                // neighbor id
                var id = np[0] + np[1] * side;

                neighbors.Add(id);
            }

            return neighbors;
        }

        /// <summary>
        /// Retruns a list of all nodes that are neighbors to the specified node
        /// and also not connected to any node in the given graph.
        /// </summary>
        /// <param name="maze">Graph with all the nodes.</param>
        /// <param name="node">Node to find unconeccted neighbors for.</param>
        /// <returns>A list with all unconeccted neighbors.</returns>
        private static List<int> GetUnconecctedNeigbors(Graph maze, int node)
        {
            // get all negibors
            var neigbors = GetNeighbors(maze, node);

            // remove neigbors that is not connected to any other node
            neigbors.RemoveAll(x => maze.AdjacencyList[x].Count > 0);

            return neigbors;
        }
    }
}
