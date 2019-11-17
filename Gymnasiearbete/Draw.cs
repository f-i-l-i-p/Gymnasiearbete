using System;
using System.Linq;

namespace Gymnasiearbete
{
    static class Draw
    {
        public static void GraphMaze(Graph graph, int w, int h, int[] path = null, int[] explored = null)
        {
            // Returns the node id at a given coordinate
            int Id(int x, int y)
            {
                return x + y * w;
            }

            string previousLine = null;

            for (int y = 0; y < h; y++)
            {
                string line = string.Empty;
                string nextLine = string.Empty;


                // Line with nodes (nextLine)
                for (int x = 0; x < w; x++)
                {
                    if (path != null && path.Contains(Id(x, y)))
                        nextLine += "P";
                    else if (explored != null && explored.Contains(Id(x, y)))
                        nextLine += "E";
                    else
                        nextLine += "O";

                    // If edge on right side
                    if (Id(x, y) + 1 < graph.AdjacencyList.Count && graph.AdjacencyList[Id(x, y)].Contains(Id(x, y) + 1))
                        nextLine += "---";
                    else
                        nextLine += "   ";
                }

                // Line without nodes (only edges) (line)
                for (int x = 0; x < w; x++)
                {
                    // If edge under
                    if (Id(x, y) - w > 0 && graph.AdjacencyList[Id(x, y)].Contains(Id(x, y) - w))
                        line += "|";
                    else
                        line += " ";
                    line += "   ";
                }

                // Print line without nodes (line)
                if (previousLine != null)
                {
                    for (int i = 0; i < line.Length; i += 4)
                    {
                        if (line[i] == ' ')
                        {
                            Console.Write("    ");
                            continue;
                        }

                        if (previousLine[i] == 'P' && nextLine[i] == 'P')
                            Console.ForegroundColor = ConsoleColor.Red;
                        else if (previousLine[i] == 'E' && nextLine[i] == 'E')
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        else
                            Console.ForegroundColor = ConsoleColor.White;

                        Console.Write(line[i] + "   ");
                    }
                }
                Console.WriteLine();

                // Print line with nodes (nextLine)
                for (int i = 0; i < nextLine.Length; i += 4)
                {
                    if (nextLine[i] == 'P')
                        Console.ForegroundColor = ConsoleColor.Red;
                    else if (nextLine[i] == 'E')
                        Console.ForegroundColor = ConsoleColor.Yellow;
                    else
                        Console.ForegroundColor = ConsoleColor.White;

                    Console.Write(nextLine[i]);

                    if (i < nextLine.Length - 1 && nextLine[i + 1] == '-')
                    {
                        if (nextLine[i] == 'P' && nextLine[i] + 4 == 'P')
                            Console.ForegroundColor = ConsoleColor.Red;
                        else if (nextLine[i] == 'E' && nextLine[i] + 4 == 'E')
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("---");
                    }
                    else
                        Console.Write("   ");
                }
                Console.WriteLine();


                previousLine = nextLine;
            }

            Console.ResetColor();
        }
    }
}
