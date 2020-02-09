using Gymnasiearbete.Graphs;
using Gymnasiearbete.MazeGeneration;
using Gymnasiearbete.Test;
using Gymnasiearbete.UI;
using System;
using System.Linq;
using static Gymnasiearbete.Pathfinding.GraphPruning;
using static Gymnasiearbete.Pathfinding.Pathfinder;

namespace Gymnasiearbete
{
    class Program
    {
        static void Main(string[] args)
        {
            var opptions = new string[] { "Run Tests", "Regenerate Graphs", "Test graph generation", "Exit" };
            int selected;
            while ((selected = Menu.Create(opptions)) != opptions.Length - 1)
            {
                switch (selected)
                {
                    case 0: // Run Tests
                        TestManager.RunTests(
                            new PruningAlgorithm[] {PruningAlgorithm.None, PruningAlgorithm.CornerJumps },
                            new PathfindingAlgorithm[] { PathfindingAlgorithm.BFS, PathfindingAlgorithm.Dijkstras, PathfindingAlgorithm.AStar },
                            10);
                        break;
                    case 1: // Regenerate Graphs
                        GraphManager.RegenerateGraphs(
                            new double[11].Select((val, index) => index * 0.1),
                            new int[16].Select((val, index) => (index + 1) * 8),
                            800);
                        break;
                    case 2: // Test graph generation
                        Console.WriteLine(0);
                        Draw.GraphMaze(MazeGenerator.GenerateMaze(8, 0));
                        Console.WriteLine(0.5);
                        Draw.GraphMaze(MazeGenerator.GenerateMaze(8, 0.5));
                        Console.WriteLine(1);
                        Draw.GraphMaze(MazeGenerator.GenerateMaze(8, 1));
                        break;
                }
            }
        }
    }
}
