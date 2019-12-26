using Gymnasiearbete.Graphs;
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

            return graph;
        }

        /// <summary>
        /// Reduces the complexity of a maze by adding random edges between nodes.
        /// If complexity == 0, then all nodes will be connected.
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
            // Loop through all nodes
            foreach (var node in maze.Nodes)
            {
                // get all neighbors around the node
                var nbs = GetNeighbors(maze, node);

                // for each neighbor
                foreach (var neighbor in nbs)
                {
                    // If already connected to node
                    if (node.Adjacents.Exists(x => x.Id == neighbor.Id))
                        continue;

                    // Probability that it connects with the node: (1 - complexity).
                    if (random.NextDouble() >= complexity)
                        // connect node and neighbor
                        maze.AddEdge(node, neighbor);
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
            var maze = new Graph();

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
                    maze.AddNode(new Position
                    {
                        X = x,
                        Y = y,
                    });
                }
            }


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
            var nodeStack = new Stack<Node>();
            // add a start node
            nodeStack.Push(maze.Nodes[0]);

            while (nodeStack.Any())
            {
                // get current node from stack
                var currentNode = nodeStack.Peek();

                // get all unvisited neighbors around the current node
                var unbs = GetUnconecctedNeigbors(maze, currentNode);

                // If there are any unvisited neighbors
                if (unbs.Any())
                {
                    // chose a random neighbor as the next node
                    var nextNode = unbs[random.Next(0, unbs.Count)];

                    // connect the nodes
                    maze.AddEdge(currentNode, nextNode);

                    // add the new node to the stack
                    nodeStack.Push(nextNode);
                }
                else
                {
                    // remove the current node from the stack
                    nodeStack.Pop();
                }
            }

            return maze;
        }

        /// <summary>
        /// Returns a list of all nodes that are neighbors to the specified node
        /// (beside the specified node if the graph is an even square) in the given graph.
        /// </summary>
        /// <param name="maze">Graph with all the nodes.</param>
        /// <param name="node">Node to find neighbors for.</param>
        /// <returns>A list with all neighbors.</returns>
        private static List<Node> GetNeighbors(Graph maze, Node node)
        {
            // side length of the maze
            int side = (int)Math.Sqrt(maze.Nodes.Count);

            // list of all possible neighbor positions
            var possitions = new List<Position>
            {
                new Position { X = node.Position.X    , Y = node.Position.Y - 1},
                new Position { X = node.Position.X + 1, Y = node.Position.Y    },
                new Position { X = node.Position.X    , Y = node.Position.Y + 1},
                new Position { X = node.Position.X - 1, Y = node.Position.Y    },
            };

            var neighbors = new List<Node>();

            // for each possible neighbor position
            foreach (var position in possitions)
            {
                // If neighbor is outside graph
                if (position.X < 0 || position.X >= side || position.Y < 0 || position.Y >= side)
                    continue;

                // find neighbor
                var neigbor = maze.Nodes[position.X + position.Y * side];

                neighbors.Add(neigbor);
            }

            return neighbors;
        }

        /// <summary>
        /// Returns a list of all nodes that are neighbors to the specified node
        /// and also not connected to any node in the given graph.
        /// </summary>
        /// <param name="maze">Graph with all the nodes.</param>
        /// <param name="node">Node to find unconnected neighbors for.</param>
        /// <returns>A list with all unconnected neighbors.</returns>
        private static List<Node> GetUnconecctedNeigbors(Graph maze, Node node)
        {
            // get all neighbors
            var neigbors = GetNeighbors(maze, node);

            // remove neighbors that is not connected to any other node
            neigbors.RemoveAll(x => x.Adjacents.Count > 0);

            return neigbors;
        }
    }
}
