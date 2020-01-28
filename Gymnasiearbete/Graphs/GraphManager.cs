using Gymnasiearbete.MazeGeneration;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Gymnasiearbete.Graphs
{
    static class GraphManager
    {
        public static readonly string saveLocation = Path.GetFullPath(@"..\..\..\SavedGraphs");

        public static void RegenerateGraphs(IEnumerable<double> complexities, IEnumerable<int> sizes, int sizeRepeat)
        {
            // Remove old graph directory
            if (Directory.Exists(saveLocation))
                Directory.Delete(saveLocation, true);

            var totalGraphsCount = complexities.Count() * sizes.Count() * sizeRepeat;
            var addedGraphsCount = 0;

            // For each graph complexity
            foreach (var complexity in complexities)
            {
                // For each graph side size
                foreach (var size in sizes)
                {
                    // print progress
                    System.Console.WriteLine($"{string.Format("{0:0.00}", 100 * (double)addedGraphsCount / totalGraphsCount)}%");

                    // For each graph size repeat
                    for (int repeat = 0; repeat < sizeRepeat; repeat++)
                    {
                        // generate maze
                        var maze = MazeGenerator.GenerateMaze(size, complexity);

                        // folder path to save graph
                        string folderPath = $"{saveLocation}/{complexity}/{size}";

                        // saves as {saveLocaation}/{complexity}/{size}/{repeat}.json
                        Save(maze, folderPath, repeat.ToString());

                        addedGraphsCount++;
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

