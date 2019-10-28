using System.Collections.Generic;

namespace Gymnasiearbete.Pathfinding
{
    class Pathfinder
    {
        public enum SearchType { BFS, TEST };

        public Graph Graph { get; }
        public int Source { get; }
        public int Destination { get; }

        public Pathfinder(Graph graph, int source, int destination)
        {
            Graph = graph;
            Source = source;
            Destination = destination;
        }

        public bool FindPath(SearchType searchType, out List<int> path)
        {
            switch (searchType)
            {
                case SearchType.BFS:
                    return BFS(out path);
                case SearchType.TEST:
                    return TEST(out path);
                default:
                    path = null;
                    return false;
            }
        }

        public bool TEST(out List<int> path)
        {
            return BFS(out path);
        }

        public bool BFS(out List<int> path)
        {
            var queue = new List<int> { Source };
            var visited = new bool[Graph.AdjacencyList.Count];
            var parent = new int[Graph.AdjacencyList.Count];

            visited[Source] = true;

            while (queue.Count > 0)
            {
                // Pop first node from queue
                var n = queue[0];
                queue.RemoveAt(0);

                // Check if it is the destination node
                if (n == Destination)
                {
                    // Create path from parent[]
                    path = new List<int>();
                    var currentNode = Destination;
                    do
                    {
                        path.Add(currentNode);
                        currentNode = parent[currentNode];
                    } while (currentNode != Source);
                    return true;
                }

                // Add all unvisited nodes to the queue & assign their parent
                foreach (var e in Graph.AdjacencyList[n])
                {
                    // if adjacent node s not visited
                    if (!visited[e])
                    {
                        visited[e] = true;
                        parent[e] = n;
                        queue.Add(e);
                    }
                }
            }

            // no solution found
            path = null;
            return false;
        }
    }
}
