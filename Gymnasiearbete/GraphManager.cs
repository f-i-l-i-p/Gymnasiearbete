using Gymnasiearbete.MazeGeneration;
using System.Collections.Generic;
using System.IO;

namespace Gymnasiearbete
{
    static class GraphManager
    {
        public static string saveLocation = Path.GetFullPath(@"..\..\..\SavedGraphs");

        public enum MazeType { Perfect, NonPerfect }

        // Generates and saves Graphs
        public static void RegenrateGraphs(MazeType mazeType, int sizeStart, int sizeEnd, int sizeIncrease, int sizeCount)
        {
            // path to the folder with {mazeType} graphs
            string graphFolderPath = $"{saveLocation}/{mazeType.ToString()}";

            // For each graph size
            for (int size = sizeStart; size <= sizeEnd ; size *= sizeIncrease)
            {
                // Repeat size count
                for (int i = 0; i < sizeCount; i++)
                {
                    Cell[,] maze;

                    // Generate maze
                    if (mazeType == MazeType.Perfect)
                        maze = MazeGenerator.GeneratePerfectMaze(size, size);
                    else if (mazeType == MazeType.NonPerfect)
                        maze = MazeGenerator.GenerateNonPerfectMaze(size, size, 0.9);
                    else
                        throw new System.ArgumentException($"{mazeType.ToString()} is not a possible maze type", "mazeType");

                    // convert to Graph & save
                    // saves in folder {saveLocaation}/{size} as {sizeCount}.json
                    Save(MazeGraphConverter.ToGraph(maze), $"{graphFolderPath}/{size.ToString()}", i.ToString());
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
            var directories = Directory.GetDirectories($"{saveLocation}/{MazeType.Perfect.ToString()}");
            var paths = new List<string[]>();

            // For each graph size
            foreach (var directory in directories)
            {
                // add all paths in directory
                paths.Add(Directory.GetFiles(directory));
            }

            return paths;
        }
    }
}

