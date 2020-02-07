using Gymnasiearbete.Helpers;
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

        /// <summary>
        /// Counts all the saved graphs in saveLocation.
        /// </summary>
        /// <returns>The number of saved graphs.</returns>
        public static int CountSavedGraphs()
        {
            return Directory.GetFiles(saveLocation, "*.graph", SearchOption.AllDirectories).Length;
        }

        /// <summary>
        /// Saves a graph to the disk.
        /// </summary>
        /// <param name="graph">Graph to be saved.</param>
        /// <param name="directory">Directory to save in.</param>
        /// <param name="name">Filename without extension.</param>
        public static void Save(Graph graph, string directory, string name)
        {
            // creates a new directory if the directory does not already exist
            Directory.CreateDirectory(directory);

            // convert graph to json and compress
            var graphStr = StringCompression.Compress(Newtonsoft.Json.JsonConvert.SerializeObject(graph));

            // save
            File.WriteAllText($"{directory}//{name}.graph", graphStr);
        }

        /// <summary>
        /// Loads a graph from the disk.
        /// </summary>
        /// <param name="path">Full path to the graph file.</param>
        /// <returns>A graph loaded from the specified path.</returns>
        public static Graph Load(string path)
        {
            if (!File.Exists(path))
                throw new System.ArgumentException($"The path \"{path}\" does not exist", "path");

            // read and decompress
            var graphStr = StringCompression.Decompress(File.ReadAllText(path));

            // TODO: error handling
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Graph>(graphStr);
        }
    }
}

