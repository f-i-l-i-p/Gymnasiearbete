using System;

namespace Gymnasiearbete
{
    static class Draw
    {
        public static void GraphMaze(Graph graph, int w, int h)
        {
            // Returns the node id at a given coordinate
            int Id(int x, int y)
            {
                return x + y * w;
            }

            for (int y = 0; y < h; y++)
            {
                string line = string.Empty;

                // Line with nodes
                for (int x = 0; x < w; x++)
                {
                    line += "O";
                    // If edge on right side
                    if (Id(x, y) + 1 < graph.AdjacencyList.Count && graph.AdjacencyList[Id(x, y)].Contains(Id(x, y) + 1))
                        line += "---";
                    else
                        line += "   ";
                }

                Console.WriteLine(line);
                line = string.Empty;

                // Line without nodes (only edges)
                for (int x = 0; x < w; x++)
                {
                    // If edge under
                    if (Id(x, y) + w < graph.AdjacencyList.Count && graph.AdjacencyList[Id(x, y)].Contains(Id(x, y) + w))
                        line += "|";
                    else
                        line += " ";
                    line += "   ";
                }

                Console.WriteLine(line);
            }
        }
    }
}
