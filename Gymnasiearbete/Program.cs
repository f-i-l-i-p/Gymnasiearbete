using Gymnasiearbete.Graphs;
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
                        Test.TestManager.RunTests(
                            new OptimizationType[] {OptimizationType.None, OptimizationType.CornerJumps }, 
                            new SearchType[] { SearchType.BFS, SearchType.Dijkstras, SearchType.AStar },
                            10);
                        break;
                    case 1: // Regenerate Graphs
                        GraphManager.RegenerateGraphs(
                            new double[11].Select((val, index) => index * 0.1),
                            new int[10].Select((val, index) => (index + 1) * 8),
                            1);
                        break;
                    case 2:
                        System.Console.WriteLine(0);
                        Draw.GraphMaze(MazeGeneration.MazeGenerator.GenerateMaze(8, 0));
                        System.Console.WriteLine(0.5);
                        Draw.GraphMaze(MazeGeneration.MazeGenerator.GenerateMaze(8, 0.5));
                        System.Console.WriteLine(1);
                        Draw.GraphMaze(MazeGeneration.MazeGenerator.GenerateMaze(8, 1));
                        break;
                }
            }
        }
    }
}
