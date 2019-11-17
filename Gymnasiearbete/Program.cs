namespace Gymnasiearbete
{
    class Program
    {
        static void Main(string[] args)
        {
            var opptions = new string[] { "Run Tests", "Regenerate Graphs", "View Graphs", "Test graph generation", "Exit" };
            int selected;
            while ((selected = Menu.Create(opptions)) != opptions.Length - 1)
            {
                switch (selected)
                {
                    case 0: // Run Tests
                        Test.Run();
                        break;
                    case 1: // Regenerate Graphs
                        GraphManager.RegenrateGraphs(GraphManager.MazeType.Perfect, 16, 64, 2, 10);
                        GraphManager.RegenrateGraphs(GraphManager.MazeType.NonPerfect, 16, 64, 2, 10);
                        break;
                    case 2: // View Graphs
                        var graph = GraphManager.Load(GraphManager.saveLocation + "\\p\\16[0].json");
                        Draw.GraphMaze(graph, 16, 16);
                        break;
                    case 3:
                        var g = MazeGeneration.MazeGenerator.GenerateMaze(25, 0);
                        Draw.GraphMaze(g, 5, 5);
                        break;
                }
            }
        }
    }
}
