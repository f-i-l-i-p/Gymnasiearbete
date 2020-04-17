using System;
using Gymnasiearbete.Graphs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gymnasiearbete.UnitTests
{
    [TestClass]
    public class MazeTests
    {
        [TestMethod]
        public void GenerateMaze_RandomInputParameters_ReturnsMazesWithCorrectSize()
        {
            var rnd = new Random();

            for (int i = 0; i < 100; i++)
            {
                int side = rnd.Next(1, 100);
                var maze = MazeGeneration.MazeGenerator.GenerateMaze(side, rnd.NextDouble());

                Assert.AreEqual(side * side, maze.Nodes.Count);
            }
        }

        [TestMethod]
        public void GenerateMaze_RandomInputParameters_ReturnsSolvableMazes()
        {
            var rnd = new Random();

            for (int i = 0; i < 100; i++)
            {
                int side = rnd.Next(1, 100);
                int startNodeId = rnd.Next(0, side * side - 1);
                int destinationNodeId = rnd.Next(0, side * side - 1);
                var maze = MazeGeneration.MazeGenerator.GenerateMaze(side, rnd.NextDouble());

                Assert.IsTrue(IsSolvable(maze, startNodeId, destinationNodeId));
            }
        }

        /// <summary>
        /// Uses BFS algorithm to check if it is possible to navigate from the start node to the destination node.
        /// </summary>
        /// <param name="maze">Maze to check.</param>
        /// <param name="startNodeId">Start node id.</param>
        /// <param name="destinationNodeId">Destination node id.</param>
        /// <returns>Returns boolean indicating if it is solvable.</returns>
        private bool IsSolvable(Graph maze, int startNodeId, int destinationNodeId)
        {
            var queue = new System.Collections.Generic.Queue<Node>();
            var visited = new bool[maze.Nodes.Count + 500];

            // Add Source as a start node
            queue.Enqueue(maze.Nodes[startNodeId]);
            visited[startNodeId] = true;

            // While there are nodes to check
            while (queue.Count > 0)
            {
                // dequeue the first node in the queue
                var current = queue.Dequeue();

                // Check if it is the destination node
                if (current.Id == destinationNodeId)
                    return true;

                // Loop through all neighbors
                foreach (var adjacent in current.Adjacents)
                {
                    //If this adjacent node is unvisited
                    if (!visited[adjacent.Id])
                    {
                        visited[adjacent.Id] = true;
                        queue.Enqueue(maze.Nodes[adjacent.Id]);
                    }

                }
            }

            // no solution found
            return false;
        }
    }
}
