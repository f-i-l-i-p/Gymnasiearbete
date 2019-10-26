namespace Gymnasiearbete
{
    class Program
    {
        static void Main(string[] args)
        {
            var opptions = new string[] { "Run Tests", "Regenerate Graphs", "View Graphs", "Exit" };
            int selected;
            while ((selected = Menu.Create(opptions)) != opptions.Length - 1)
            {
                switch (selected)
                {
                    case 0: // Run Tests
                        Test.Run();
                        break;
                    case 1: // Regenerate Graphs
                        GraphManager.RegenrateGraphs(true, 16, 64, 16, 10, "p");
                        GraphManager.RegenrateGraphs(false, 16, 64, 16, 10, "n");
                        break;
                    case 2: // View Graphs
                        var graph = GraphManager.Load(GraphManager.saveLocation + "\\p\\16[0].json");
                        Draw.GraphMaze(graph, 16, 16);
                        break;
                }
            }
        }
    }
}
