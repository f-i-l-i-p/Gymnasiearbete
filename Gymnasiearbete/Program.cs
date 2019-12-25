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
                        Test.TestManager.RunTests();
                        break;
                    case 1: // Regenerate Graphs
                        GraphManager.RegenerateGraphs();
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
