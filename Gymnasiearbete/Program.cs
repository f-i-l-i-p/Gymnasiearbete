﻿namespace Gymnasiearbete
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
                        Test.TestManager.RunTests();
                        break;
                    case 1: // Regenerate Graphs
                        GraphManager.RegenerateGraphs();
                        break;
                    case 2:
                        var g = MazeGeneration.MazeGenerator.GenerateMaze(64, 0);
                        Draw.GraphMaze(g);
                        break;
                }
            }
        }
    }
}
