using Gymnasiearbete.MazeGeneration;
using System.Collections.Generic;
using System.IO;

namespace Gymnasiearbete
{
    static class GraphManager
    {
        public static string saveLocation = Path.GetFullPath(@"..\..\..\SavedGraphs");

        // Generates and saves Graphs
        public static void RegenrateGraphs(float openness, int sizeStart, int sizeEnd, int sizeIncrease, int sizeCount)
        {
            // remove old graph directory
            Directory.Delete(saveLocation, true);

            // path to the folder with {mazeType} graphs
            string graphFolderPath = $"{saveLocation}/{openness.ToString()}";

            // For each graph size
            for (int size = sizeStart; size <= sizeEnd ; size *= sizeIncrease)
            {
                // Repeat size count
                for (int i = 0; i < sizeCount; i++)
                {
                    // Generate maze
                    var maze = MazeGeneration.MazeGenerator.GenerateMaze(size, openness);

                    // Save
                    // saves in folder {saveLocaation}/{size} as {sizeCount}.json
                    Save(maze, $"{graphFolderPath}/{size.ToString()}", i.ToString());
                }
            }
        }

        // Saves a Graph
        public static void Save(Graph graph, string folderPath, string name)
        {
            // creates a new directory if the directory does not already exist
            Directory.CreateDirectory(folderPath);

            // conver graph to json
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(graph);
            // save
            File.WriteAllText($"{folderPath}//{name}.json", str);
        }

        // Loads a Graph
        public static Graph Load(string path)
        {
            if (!File.Exists(path))
                throw new System.ArgumentException($"The paht \"{path}\" does not exist", "path");

            var obj = File.ReadAllText(path);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Graph>(obj);
        }

        /// <summary>
        /// Returns paths to all saved perfect graphs.
        /// </summary>
        /// <returns>Paths to all perfect graphs.</returns>
        public static List<string[]> GetAllPerfectGraphPaths()
        {
            var typeDirectories = Directory.GetDirectories(saveLocation);
            var paths = new List<string[]>();

            // For each graph type
            foreach (var typeDirecotrie in typeDirectories)
            {
                var directories = Directory.GetDirectories(typeDirecotrie);
                // For each graph size
                foreach (var directory in directories)
                {
                    // add all paths in directory
                    paths.Add(Directory.GetFiles(directory));
                }
            }

            return paths;
        }
    }
}

