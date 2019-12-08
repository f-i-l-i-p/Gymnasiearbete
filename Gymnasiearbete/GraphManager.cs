using Gymnasiearbete.MazeGeneration;
using System;
using System.Collections.Generic;
using System.IO;

namespace Gymnasiearbete
{
    static class GraphManager
    {
        public static readonly string saveLocation = Path.GetFullPath(@"..\..\..\SavedGraphs");

        // Generates and saves Graphs
        public static void RegenerateGraphs(float openness, int sizeStart, int sizeEnd, int sizeIncrease, int sizeCount)
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


        public static void RegenerateGraphs()
        {
            // remove old graph directory
            Directory.Delete(saveLocation, true);

            // For each graph openness
            for (double openness = 0; openness <= 1; openness += 0.1d)
            {
                // For each graph size
                // sizes: 64, 144, 256, 400, 576, 784, 1024, 1296, 1600, 1936, 2304, 2704, 3136, 3600, 4096.
                for (int size = 64; size <= 4096; size = (int)Math.Pow(Math.Sqrt(size) + 4, 2))
                {
                    // For each graph size repeat
                    for (int repeat = 0; repeat < 5; repeat++)
                    {
                        // generate maze
                        var maze = MazeGenerator.GenerateMaze(size, openness);

                        // Save
                        string folderPath = $"{saveLocation}/{openness}/{size}";
                        // saves as {saveLocaation}/{openness}/{size}/{repeat}.json
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

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Graph>(obj);
        }
    }
}

