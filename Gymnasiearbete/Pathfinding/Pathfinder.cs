using System;
using System.Collections.Generic;
using System.Linq;
using Gymnasiearbete.Pathfinding.QueueNodes;
using Priority_Queue;

namespace Gymnasiearbete.Pathfinding
{
    class Pathfinder
    {
        /// <summary>
        /// All path-finding algorithms that can be used in this class.
        /// </summary>
        public enum SearchType { BFS, Dijkstras, AStar };

        /// <summary>
        /// Graph to search through.
        /// </summary>
        public Graph Graph { get; }
        /// <summary>
        /// The source/start node when searching for the shortest path.
        /// </summary>
        public int Source { get; }
        /// <summary>
        /// The destination/end node when searching for the shortest path.
        /// </summary>
        public int Destination { get; }

        public Pathfinder(Graph graph, int source, int destination)
        {
            Graph = graph;
            Source = source;
            Destination = destination;
        }

        /// <summary>
        /// Searches the graph with the specified searchType algorithm to find the shortest path from the source node to the destination node.
        /// </summary>
        /// <param name="searchType">Search algorithm to use.</param>
        /// <param name="path">A list of steps to take to travel from the source node to the destination node.</param>
        /// <returns>A boolean indicating if the path was found.</returns>
        public bool FindPath(SearchType searchType, out List<int> path, out int visitedNodes)
        {
            switch (searchType)
            {
                case SearchType.BFS:
                    return BFS(out path, out visitedNodes);
                case SearchType.Dijkstras:
                    return Dijkstras(out path, out visitedNodes);
                case SearchType.AStar:
                    return AStar(out path, out visitedNodes);
                default:
                    path = null;
                    visitedNodes = 0;
                    return false;
            }
        }

        // BFS, Dijkstras, A*: https://www.redblobgames.com/pathfinding/a-star/introduction.html
        // Array vs List vs LinkedList: https://i.stack.imgur.com/iBz6V.png

        /// <summary>
        /// Searches the graph with the Breadth First Search algorithm to find the shortest path from the source node to the destination node.
        /// </summary>
        /// <param name="visitedNodes">The total number of nodes visited.</param>
        /// <param name="path">A list of steps to take to travel from the source node to the destination node.</param>
        /// <returns>A boolean indicating if the path was found.</returns>
        public bool BFS(out List<int> path, out int visitedNodes)
        {
            visitedNodes = 0;

            var queue = new Queue<int>();
            var parent = new int?[Graph.AdjacencyList.Count];

            // Add Source to queue & set Source parent
            queue.Enqueue(Source);
            parent[Source] = Source;

            // While there are nodes to check
            while (queue.Count > 0)
            {
                // dequeue the first node in the queue
                var u = queue.Dequeue();
                visitedNodes++;

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
                    if (parent[v.Id] == null)
                    {
                        parent[v.Id] = u;
                        queue.Enqueue(v.Id);
                    }
                }
            }

            // No solution found
            path = null;
            return false;
        }

        /// <summary>
        /// Searches the graph with Dijkstra's algorithm to find the shortest path from the source node to the destination node.
        /// </summary>
        /// <param name="visitedNodes">The total number of nodes visited.</param>
        /// <param name="path">A list of steps to take to travel from the source node to the destination node.</param>
        /// <returns>A boolean indicating if the path was found.</returns>
        public bool Dijkstras(out List<int> path, out int visitedNodes)
        {
            visitedNodes = 0;

            var priorityQueue = new FastPriorityQueue<FastQueueNode>(Graph.AdjacencyList.Count);
            var parent = new int?[Graph.AdjacencyList.Count];
            var cost = new float?[Graph.AdjacencyList.Count];

            // Adds a node to the priorityQueue
            void Enqueue(int node, float priority)
            {
                priorityQueue.Enqueue(new FastQueueNode(node), priority);
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
                visitedNodes++;

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
                    var newCost = cost[u] + v.Weight;

                    // If v is unvisited (i.e. v has no assigned parent) or has a higher cost than alt
                    if (cost[v.Id] == null || cost[v.Id] > newCost)
                    {
                        parent[v.Id] = u;
                        cost[v.Id] = newCost;
                        Enqueue(v.Id, newCost.Value);
                    }
                }
            }

            // No solution found
            path = null;
            return false;
        }

        /// <summary>
        /// Searches the graph with the A* algorithm to find the shortest path from the source node to the destination node.
        /// </summary>
        /// <param name="visitedNodes">The total number of nodes visited.</param>
        /// <param name="path">A list of steps to take to travel from the source node to the destination node.</param>
        /// <returns>A boolean indicating if the path was found.</returns>
        public bool AStar(out List<int> path, out int visitedNodes)
        {
            visitedNodes = 0;

            var priorityQueue = new FastPriorityQueue<FastQueueNode>(Graph.AdjacencyList.Count);
            var parent = new int?[Graph.AdjacencyList.Count];
            var gScore = new float?[Graph.AdjacencyList.Count];
            var destinationPos = Graph.NodePositions[Destination];

            // Adds a node to the priorityQueue
            void Enqueue(int node, float priority)
            {
                priorityQueue.Enqueue(new FastQueueNode(node), priority);
            }

            // Returns the distance from the node to the Destination node
            float hScore(int node)
            {
                var pos = Graph.NodePositions[node];
                return 1.0001f * Math.Abs(destinationPos.X - pos.X) + (destinationPos.Y - pos.Y);
            }

            // Add Source to the queue & set Source parent & set Source gScore
            Enqueue(Source, 0);
            parent[Source] = Source;
            gScore[Source] = 0;

            // While there are nodes in the queue
            while (priorityQueue.Any())
            {
                // dequeue the node with the lowest priority
                var u = priorityQueue.Dequeue().Value;
                visitedNodes++;

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
                    var newCost = gScore[u] + v.Weight;

                    // If v is unvisited (i.e. v has no assigned parent) or has a higher cost than newCost
                    if (gScore[v.Id] == null || gScore[v.Id] > newCost)
                    {
                        parent[v.Id] = u;
                        gScore[v.Id] = newCost;
                        Enqueue(v.Id, gScore[v.Id].Value + hScore(v.Id));
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
        /// <returns>A list on node id's from the destination node to the source node.</returns>
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
}
