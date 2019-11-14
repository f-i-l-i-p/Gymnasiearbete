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

        class Vertex
        {
            public double DistToSoruce { get; set; }
            public int Id { get; set; }
            public int Parent { get; set; }
        }

        public bool Dijkstras(out List<int> path)
        {
            var Q = new List<Vertex>();
            for (int i = 0; i < Graph.AdjacencyList.Count; i++)
            {
                Q.Add(new Vertex
                {
                    DistToSoruce = double.PositiveInfinity,
                    Id = i,
                });
            }

            // Returns the index of the node with the smalest distance to the soruce
            int IndexOfMin()
            {
                var minValue = Q[0].DistToSoruce;
                int minValueIndex = 0;

                int listLength = Q.Count;
                for (int i = 1; i < listLength; i++)
                {
                    if (Q[i].DistToSoruce < minValue)
                    {
                        minValue = Q[i].DistToSoruce;
                        minValueIndex = i;
                    }
                }

                return minValueIndex;
            }


            while (Q.Count > 0)
            {
                int index = IndexOfMin();
                Vertex u = Q[index];
                Q.RemoveAt(index);
                
                // For each neighbor of the current node
                foreach (var e in Graph.AdjacencyList[u.Id])
                {
                    // If the neigbor is unvisited
                    if (unvisited.Contains(e))
                    {

                    }
                }
            }


            //// All unvisited nodes
            //var unvisited = new List<int>();
            //for (int i = 0; i < Graph.AdjacencyList.Count; i++) unvisited.Add(i);

            //// Distance from the node i to the source
            //var distance = new double[Graph.AdjacencyList.Count];
            //Populate(distance, double.PositiveInfinity);
            //// Set source node distance to 0
            //distance[Source] = 0;


            //var parent = new int[Graph.AdjacencyList.Count];


            //while (unvisited.Count > 0)
            //{
            //    // set current node to the node with the smalest distance
            //    var u = IndexOfMin(distance);
            //    // remove from unvisited since it now will be visited
            //    unvisited.Remove(u);

            //    // For each neighbor of the current node
            //    foreach (var e in Graph.AdjacencyList[u])
            //    {
            //        // If the neigbor is unvisited
            //        if (unvisited.Contains(e))
            //        {

            //        }
            //    }
            //}
        }

        /// <summary>
        /// Populates an array with a value.
        /// </summary>
        /// <param name="arr">The array to be populated.</param>
        /// <param name="value">Value to populate the array with.</param>
        public static void Populate<T>(this T[] arr, T value)
        {
            int arrayLength = arr.Length;
            for (int i = 0; i < arrayLength; ++i)
            {
                arr[i] = value;
            }
        }

        /// <summary>
        /// Finds the index of the smallest number in a list.
        /// </summary>
        /// <typeparam name="T">Type of list objects</typeparam>
        /// <param name="list">List to search thorugh.</param>
        /// <returns>The intex of the smallest number.</returns>
        public static int IndexOfMin<T>(T[] list) where T : IComparable
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (list.Length == 0)
                throw new ArgumentException("List is empty.", "list");


            var minValue = list[0];
            int minValueIndex = 0;

            int listLength = list.Length;
            for (int i = 1; i < listLength; i++)
            {
                if (list[i].CompareTo(minValue) < 0)
                {
                    minValue = list[i];
                    minValueIndex = i;
                }
            }

            return minValueIndex;
        }
    }
}
