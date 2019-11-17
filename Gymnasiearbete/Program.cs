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
                        Test.Run();
                        break;
                    case 1: // Regenerate Graphs
                        GraphManager.RegenrateGraphs(1, 64, 64, 2, 10);
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
