using System;
using System.Collections.Generic;
using System.Linq;

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
                    path = ConstructPathFromParents(parent);
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

        public bool Dijkstras(out List<int> path)
        {
            // All unvisited nodes
            var unvisited = new List<int>();
            for (int i = 0; i < Graph.AdjacencyList.Count; i++) unvisited.Add(i);

            // Distance from the node with id i to the source
            var distance = new double[Graph.AdjacencyList.Count];
            Populate(distance, double.PositiveInfinity);

            // Set source node distance to 0
            distance[Source] = 0;


            var parent = new int[Graph.AdjacencyList.Count];


            // Returns the index of the unvisited node with the lowest distance to the source
            int MinDistNodeIndex()
            {
                var minDist = distance[unvisited[0]];
                int minDistIndex = 0;

                int unvisitedLen = unvisited.Count;
                // Loop through all unvisited nodes
                for (int i = 1; i < unvisitedLen; i++)
                {
                    // distance from the node too the source
                    var dist = distance[unvisited[i]];

                    // If it is the new minimum distance
                    if (dist < minDist)
                    {
                        minDist = dist;
                        minDistIndex = unvisited[i];
                    }
                }
                return minDistIndex;
            }


            while (unvisited.Count > 0)
            {
                // set the current node (u) to the node with the lowest distance from the source
                int index = MinDistNodeIndex();
                var u = unvisited[index];
                // remove the node from unvisited list
                unvisited.RemoveAt(index);

                // Check if this node is the destination node
                if (u == Destination)
                {
                    path = ConstructPathFromParents(parent);
                    return true;
                }

                // For each neighbor (v) of the current node
                foreach (var v in Graph.AdjacencyList[u])
                {
                    // If the neigbor is unvisited
                    if (unvisited.Contains(v)) // TODO: Optimise the if statement
                    {
                        // alt is the total distance from the source to the neighbor node.
                        // its value will be the current nodes distacne + the edge weight from the current node to the neigbor
                        var alt = distance[u] + 1;
                        
                        // If the alt distance is shorter than the nodes current distance (which will be infinite if the node is unvisited)
                        if (alt < distance[v])
                        {
                            distance[v] = alt;
                            parent[v] = u;
                        }
                    }
                }
            }

            // no solution found
            path = null;
            return false;
        }

        /// <summary>
        /// Constructs a list on node id's from the destinaation node to the source node. 
        /// </summary>
        /// <param name="parent">Array that contains all necessary nodes parents</param>
        /// <returns>A list on node id's from the destinaation node to the source node.</returns>
        private List<int> ConstructPathFromParents(int[] parent)
        {
            var path = new List<int>();
            var currentNode = Destination;
            do
            {
                path.Add(currentNode);
                currentNode = parent[currentNode];
            } while (currentNode != Source);
            return path;
        }

        /// <summary>
        /// Populates an array with a value.
        /// </summary>
        /// <param name="arr">The array to be populated.</param>
        /// <param name="value">Value to populate the array with.</param>
        public static void Populate<T>(T[] arr, T value)
        {
            int arrayLength = arr.Length;
            for (int i = 0; i < arrayLength; ++i)
            {
                arr[i] = value;
            }
        }
    }
}
