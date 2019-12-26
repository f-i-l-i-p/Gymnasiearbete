using Gymnasiearbete.MazeGeneration;
using System.IO;

namespace Gymnasiearbete
{
    static class GraphManager
    {
        public static readonly string saveLocation = Path.GetFullPath(@"..\..\..\SavedGraphs");

        public static void RegenerateGraphs()
        {
            // Remove old graph directory
            if (Directory.Exists(saveLocation))
                Directory.Delete(saveLocation, true);

            // For each graph complexity
            for (double complexity = 0; complexity <= 1; complexity += 0.1d)
            {
                // For each graph side size
                // graph sizes: 64, 144, 256, 400, 576, 784, 1024, 1296, 1600, 1936, 2304, 2704, 3136, 3600, 4096.
                for (int side = 8; side <= 64; side += 4)
                {
                    // For each graph size repeat
                    for (int repeat = 0; repeat < 5; repeat++)
                    {
                        // generate maze
                        var maze = MazeGenerator.GenerateMaze(side, complexity);

                        // folder path to save graph
                        string folderPath = $"{saveLocation}/{complexity}/{side}";

                        // saves as {saveLocaation}/{complexity}/{size}/{repeat}.json
                        Save(maze, folderPath, repeat.ToString());
                    }
                }
            }
        }

        // Saves a Graph
        public static void Save(Graph graph, string folderPath, string name)
        {
            // creates a new directory if the directory does not already exist
            Directory.CreateDirectory(folderPath);

            // convert graph to json
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(graph);
            // save
            File.WriteAllText($"{folderPath}//{name}.json", str);
        }

        // Loads a Graph
        public static Graph Load(string path)
        {
            if (!File.Exists(path))
                throw new System.ArgumentException($"The path \"{path}\" does not exist", "path");

            var obj = File.ReadAllText(path);

            // TODO: error handling
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Graph>(obj);
        }
    }
}

