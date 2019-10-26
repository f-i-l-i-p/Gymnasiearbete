using Gymnasiearbete.MazeGeneration;
using System.Collections.Generic;
using System.IO;

namespace Gymnasiearbete
{
    static class GraphManager
    {
        public static string saveLocation = Path.GetFullPath(@"..\..\..\SavedGraphs");

        // Generates and saves Graphs
        public static void RegenrateGraphs(bool perfectMaze, int sizeStart, int sizeEnd, int sizeIncrease, int sizeCount, string folderName)
        {
            string folderPath = $"{saveLocation}//{folderName}";

            if (!Directory.Exists(saveLocation))
                throw new System.ArgumentException($"The folder {folderName} can not be found in {saveLocation}.");


            var size = sizeStart;
            while (size <= sizeEnd) // loop through maze sizes
            {
                for (int i = 0; i < sizeCount; i++) // size count
                {
                    // Generate maze
                    Cell[,] maze;
                    if (perfectMaze)
                        maze = MazeGenerator.GeneratePerfectMaze(size, size);
                    else
                        maze = MazeGenerator.GenerateNonPerfectMaze(size, size, 0.9);

                    // convert to Graph & save
                    Save(MazeGraphConverter.ToGraph(maze), folderPath, $"{size}[{i}]");
                }
                size *= sizeIncrease;
            }
        }

        // Saves a Graph
        public static void Save(Graph graph, string folderPath, string name)
        {
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(graph);
            File.WriteAllText($"{folderPath}//{name}.json", str);
        }

        // Loads a Graph
        public static Graph Load(string path)
        {
            var obj = File.ReadAllText(path);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<Graph>(obj);
        }

        /// <summary>
        /// Returns paths to all saved graphs in folders under the "saveLocation" folder
        /// </summary>
        /// <returns>All graph paths</returns>
        public static List<string> GetAllGraphPaths()
        {
            var directories = Directory.GetDirectories(saveLocation);
            var paths = new List<string>();
            // TODO: change to directories.foreach...
            foreach (var directory in directories)
            {
                paths.AddRange(Directory.GetFiles(directory));
            }
            return paths;
        }
    }
}

