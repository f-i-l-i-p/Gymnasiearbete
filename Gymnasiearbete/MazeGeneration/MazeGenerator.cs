﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Gymnasiearbete.MazeGeneration
{
    static class MazeGenerator
    {
        private static Random random = new Random();

        public static Cell[,] GeneratePerfectMaze(int width, int height)
        {
            var maze = new Cell[width, height];

            // Fill maze with unvisited cells
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    maze[i, j] = new Cell
                    {
                        X = i,
                        Y = j,
                        IsVissited = false,
                    };
                }
            }

            // Returns a List with unvisited cells around a specified cell
            List<Cell> GetUnvisitedNeighbors(Cell cell)
            {
                // All possible neighbor positions relative to "cell"
                var relPos = new List<int[]>
                {
                    new int[] {  0, -1 },
                    new int[] {  1,  0 },
                    new int[] {  0,  1 },
                    new int[] { -1,  0 },
                };

                var neighbors = new List<Cell>();

                foreach (var p in relPos)
                {
                    // calculate neighbor position
                    var np = new int[] { p[0] + cell.X, p[1] + cell.Y };

                    // Check if the position is inside the maze and if it is unvisited
                    if (np[0] >= 0 && np[0] < width && np[1] >= 0 && np[1] < height
                        && !maze[np[0], np[1]].IsVissited)
                    {
                        // Save the unvisited neighbor
                        neighbors.Add(maze[np[0], np[1]]);
                    }
                }

                return neighbors;
            }

            var curentCells = new List<Cell>
            {
                maze[0, 0]
            };
            maze[0, 0].IsVissited = true;

            // while there are active cells
            while (curentCells.Count > 0)
            {
                // get the current cell from the stack
                var c = curentCells.Last();

                // get all unvisited neighbors around the current cell
                var nbs = GetUnvisitedNeighbors(c);

                // if there are any unvisited neighbors
                if (nbs.Count > 0)
                {
                    // chose a random neighbor as the next cell
                    var nextCell = nbs[random.Next(0, nbs.Count)];

                    // calculate the direction from the current cell to the next cell
                    var dir = new int[] { nextCell.X - c.X, nextCell.Y - c.Y };

                    // remove the walls between the current cell and the next cell
                    c.RemoveWall(dir);
                    nextCell.RemoveWall(dir.Select(i => -i).ToArray());

                    // mark it as visited
                    nextCell.IsVissited = true;
                    // make the next cell the current cell by adding it to the stack
                    curentCells.Add(nextCell);
                }
                else

                {
                    // remove the current cell from the stack
                    curentCells.RemoveAt(curentCells.Count - 1);
                }
            }

            return maze;
        }

        public static Cell[,] GenerateNonPerfectMaze(int width, int height, double difficulty)
        {
            var maze = GeneratePerfectMaze(width, height);

            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    if (i < maze.GetLength(0) - 1 && random.NextDouble() > difficulty)
                    {
                        maze[i, j].WallRight = false;
                        maze[i + 1, j].WallLeft = false;
                    }
                    if (j < maze.GetLength(1) - 1 && random.NextDouble() > difficulty)
                    {
                        maze[i, j].WallBottom = false;
                        maze[i, j + 1].WallTop = false;
                    }
                }
            }

            return maze;
        }


        public static Graph GenerateMaze(int size, float openness)
        {
            var graph = new Graph();

            int sideLength = (int)Math.Sqrt(size);

            List<int> GetNeighbors(int node)
            {
                // nodePos = { x, y }
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

                    // Check if the position is inside the maze
                    if (np[0] >= 0 && np[0] < sideLength && np[1] >= 0 && np[1] < sideLength)
                    {
                        // Save the unvisited neighbor
                        neighbors.Add(id);
                    }
                }

                return neighbors;
            }

            List<int> GetUnvissitedNeighbors(int node)
            {
                // nodePos = { x, y }
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

                    // Check if the position is inside the maze and if it is unvisited
                    if (np[0] >= 0 && np[0] < sideLength && np[1] >= 0 && np[1] < sideLength
                        // if unvissited (as not been conneted to another node)
                        && !graph.AdjacencyList[id].Any())
                    {
                        // Save the unvisited neighbor
                        neighbors.Add(id);
                    }
                }

                return neighbors;
            }

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
            for (int i = 0; i < Math.Pow(sideLength, 2); i++)
            {
                graph.AddNode();
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
                var unbs = GetUnvissitedNeighbors(currentNode);

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
                        var nbs = GetNeighbors(currentNode);
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
