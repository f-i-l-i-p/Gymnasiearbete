using Gymnasiearbete.MazeGeneration;

namespace Gymnasiearbete
{
    class MazeGraphConverter
    {
        /// <summary>
        /// Creates graph from Cell[,]
        /// The node id's will be increasing from left to right:
        /// 0 1 2
        /// 3 4 5
        /// 6 7 8
        /// </summary>
        /// <param name="Maze">A maze made of cells</param>
        /// <returns>A new graph</returns>
        public static Graph ToGraph(Cell[,] Maze)
        {
            var graph = new Graph();

            var w = Maze.GetLength(0);
            var h = Maze.GetLength(1);

            // Create one node for each cell
            int val;
            for (val = 0; val < w * h; val++)
            {
                graph.AddNode();
            }


            // Add edges
            val = 0;
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (!Maze[x, y].WallRight)
                        graph.AddEdge(val, val + 1);
                    if (!Maze[x, y].WallBottom)
                        graph.AddEdge(val, val + w);

                    val++;
                }
            }

            return graph;
        }
    }
}
