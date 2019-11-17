using System;
using System.Collections.Generic;
using System.Linq;
using Priority_Queue;

namespace Gymnasiearbete.Pathfinding
{
    class Pathfinder
    {
        public enum SearchType { BFS, Dijkstras2, Dijkstras, TEST };

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
                case SearchType.Dijkstras:
                    return Dijkstras(out path);
                case SearchType.Dijkstras2:
                    return Dijkstras2(out path);
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

        // BFS, Dijkstras, A*: https://www.redblobgames.com/pathfinding/a-star/introduction.html
        // Array vs List vs LinkedList: https://i.stack.imgur.com/iBz6V.png

        public bool BFS(out List<int> path)
        {
            var queue = new Queue<int>(); https://i.stack.imgur.com/iBz6V.png
            var parent = new int?[Graph.AdjacencyList.Count];

            // Add Source to queue & set Source parent
            queue.Enqueue(Source);
            parent[Source] = Source;

            // While there are nodes to check
            while (queue.Count > 0)
            {
                // dequeue the firt node in the queue
                var u = queue.Dequeue();

                // Check if it is the destination node
                if (u == Destination)
                {
                    path = ConstructPathFromParents(parent);
                    return true;
                }

                // Add all unvisited neighbors to the queue & assign their parent as the current node:
                // For each neighbor (v) of the current node (u)
                foreach (var v in Graph.AdjacencyList[u])
                {
                    // If v is unvisited (i.e. v has no assigned parent)
                    if (parent[v] == null)
                    {
                        parent[v] = u;
                        queue.Enqueue(v);
                    }
                }
            }

            // No solution found
            path = null;
            return false;
        }

        public bool Dijkstras(out List<int> path)
        {
            // All unvisited nodes
            var unvisited = new List<int>();
            for (int i = 0; i < Graph.AdjacencyList.Count; i++) unvisited.Add(i);

            // distance from each node to the source
            var distance = new double[Graph.AdjacencyList.Count];
            // set the dedault value to infinity
            Populate(distance, double.PositiveInfinity);

            var parent = new int?[Graph.AdjacencyList.Count];

            // Set source node distance & parent
            distance[Source] = 0;
            parent[Source] = Source;

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

            // While there are nodes to check
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
                        // its value will be the current nodes distacne + the edge weight from the current node to the neighbor
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

        public bool Dijkstras2(out List<int> path)
        {
            var priorityQueue = new FastPriorityQueue<QueueNode>(Graph.AdjacencyList.Count);
            var parent = new int?[Graph.AdjacencyList.Count];
            var cost = new int?[Graph.AdjacencyList.Count];

            // Adds a node to the priorityQueue
            void Enqueue(int node, float priority)
            {
                priorityQueue.Enqueue(new QueueNode(node), priority);
            }

            // Add Source to the queue & set Source parent & set Source cost
            Enqueue(Source, 0);
            parent[Source] = Source;
            cost[Source] = 0;

            // While there are nodes in the queue
            while (priorityQueue.Any())
            {
                // dequeue the node with the lowest priority
                var u = priorityQueue.Dequeue().Value;

                // Check if it is the destination node
                if (u == Destination)
                {
                    path = ConstructPathFromParents(parent);
                    return true;
                }

                // Add all unvisited neighbors to the queue & assign their parent as the current node if it is unvisited or will give it a lower cost:
                // For each neighbor (v) of the current node (u)
                foreach (var v in Graph.AdjacencyList[u])
                {
                    // newCost is the total cost from the source to the neighbor node.
                    // its value will be the current nodes cost + the edge weight from the current node to the neighbor
                    var newCost = cost[u] + 1;

                    // If v is unvisited (i.e. v has no assigned parent) or has a higher cost than alt
                    if (cost[v] == null || cost[v] > newCost)
                    {
                        parent[v] = u;
                        cost[v] = newCost;
                        Enqueue(v, newCost.Value);
                    }
                }
            }

            // No solution found
            path = null;
            return false;
        }

        /// <summary>
        /// Constructs a list on node id's from the destination node to the source node. 
        /// </summary>
        /// <param name="parent">Array that contains all necessary nodes parents</param>
        /// <returns>A list on node id's from the destinaation node to the source node.</returns>
        private List<int> ConstructPathFromParents(int?[] parent)
        {
            var path = new List<int>();
            var currentNode = Destination;
            do
            {
                if (parent[currentNode] == null)
                    break;

                path.Add(currentNode);
                currentNode = parent[currentNode].Value;
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

    class QueueNode : FastPriorityQueueNode
    {
        public int Value { get; set; }
        public QueueNode(int value)
        {
            Value = value;
        }
    }
}
