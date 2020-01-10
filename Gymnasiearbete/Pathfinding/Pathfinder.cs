using System;
using System.Collections.Generic;
using System.Linq;
using Gymnasiearbete.Graphs;
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
        public Node Source { get; }
        /// <summary>
        /// The destination/end node when searching for the shortest path.
        /// </summary>
        public Node Destination { get; }

        public Pathfinder(Graph graph, Node source, Node destination)
        {
            Graph = graph;
            Source = source;
            Destination = destination;
        }

        /// <summary>
        /// Searches the graph with the specified searchType algorithm to find the shortest path from the source node to the destination node.
        /// </summary>
        /// <param name="searchType">Search algorithm to use.</param>
        /// <returns>A list of steps to take to travel from the source node to the destination node.
        ///          If no path is found, null is returned.</returns>
        public List<Node> FindPath(SearchType searchType, out int visitedNodes)
        {
            switch (searchType)
            {
                case SearchType.BFS:
                    return BFS(out visitedNodes);
                case SearchType.Dijkstras:
                    return Dijkstras(out visitedNodes);
                case SearchType.AStar:
                    return AStar(out visitedNodes);
                default:
                    throw new NotImplementedException($"{searchType.ToString()} is not implemented");
            }
        }

        // BFS, Dijkstras, A*: https://www.redblobgames.com/pathfinding/a-star/introduction.html
        // Array vs List vs LinkedList: https://i.stack.imgur.com/iBz6V.png

        /// <summary>
        /// Searches the graph with the Breadth First Search algorithm to find the shortest path from the source node to the destination node.
        /// </summary>
        /// <param name="visitedNodes">The total number of nodes visited.</param>
        /// <returns>A list of steps to take to travel from the source node to the destination node.
        ///          If no path is found, null is returned.</returns>
        public List<Node> BFS(out int visitedNodes)
        {
            visitedNodes = 0;

            var queue = new Queue<Node>();
            var parent = new Node[Graph.Nodes.Count];

            // Add Source to queue & set Source parent
            queue.Enqueue(Source);
            parent[Source.Id] = Source;

            // While there are nodes to check
            while (queue.Count > 0)
            {
                // dequeue the first node in the queue
                var u = queue.Dequeue();
                visitedNodes++;

                // Check if it is the destination node
                if (u.Id == Destination.Id)
                    return ConstructPathFromParents(parent);

                // Add all unvisited neighbors to the queue & assign their parent as the current node:
                // For each neighbor (v) of the current node (u)
                foreach (var v in u.Adjacents)
                {
                    // If v is unvisited (i.e. v has no assigned parent)
                    if (parent[v.Id] == null)
                    {
                        parent[v.Id] = u;
                        queue.Enqueue(Graph.Nodes[v.Id]);
                    }
                }
            }

            // No solution found
            return null;
        }

        /// <summary>
        /// Searches the graph with Dijkstra's algorithm to find the shortest path from the source node to the destination node.
        /// </summary>
        /// <param name="visitedNodes">The total number of nodes visited.</param>
        /// <returns>A list of steps to take to travel from the source node to the destination node.
        ///          If no path is found, null is returned.</returns>
        public List<Node> Dijkstras(out int visitedNodes)
        {
            visitedNodes = 0;

            var priorityQueue = new FastPriorityQueue<FastQueueNode>(Graph.Nodes.Count);
            var parent = new Node[Graph.Nodes.Count];
            var cost = new float?[Graph.Nodes.Count];

            // Adds a node to the priorityQueue
            void Enqueue(Node node, float priority)
            {
                priorityQueue.Enqueue(new FastQueueNode(node), priority);
            }

            // Add Source to the queue & set Source parent & set Source cost
            Enqueue(Source, 0);
            parent[Source.Id] = Source;
            cost[Source.Id] = 0;

            // While there are nodes in the queue
            while (priorityQueue.Any())
            {
                // dequeue the node with the lowest priority
                var u = priorityQueue.Dequeue().Value;
                visitedNodes++;

                // Check if it is the destination node
                if (u == Destination)
                    return ConstructPathFromParents(parent);

                // Add all unvisited neighbors to the queue & assign their parent as the current node if it is unvisited or will give it a lower cost:
                // For each neighbor (v) of the current node (u)
                foreach (var v in u.Adjacents)
                {
                    // newCost is the total cost from the source to the neighbor node.
                    // its value will be the current nodes cost + the edge weight from the current node to the neighbor
                    var newCost = cost[u.Id] + v.Weight;

                    var neighborCost = cost[v.Id];
                    // If v is unvisited (i.e. v has no assigned cost) or has a higher cost than newCost
                    if (neighborCost == null || neighborCost > newCost)
                    {
                        parent[v.Id] = u;
                        cost[v.Id] = newCost;
                        Enqueue(Graph.Nodes[v.Id], newCost.Value);
                    }
                }
            }

            // No solution found
            return null;
        }

        /// <summary>
        /// Searches the graph with the A* algorithm to find the shortest path from the source node to the destination node.
        /// </summary>
        /// <param name="visitedNodes">The total number of nodes visited.</param>
        /// <returns>A list of steps to take to travel from the source node to the destination node.
        ///          If no path is found, null is returned.</returns>
        public List<Node> AStar(out int visitedNodes)
        {
            visitedNodes = 0;

            var priorityQueue = new FastPriorityQueue<FastQueueNode>(Graph.Nodes.Count);
            var parent = new Node[Graph.Nodes.Count];
            var gScore = new float?[Graph.Nodes.Count];

            // Adds a node to the priorityQueue
            void Enqueue(Node node, float priority)
            {
                priorityQueue.Enqueue(new FastQueueNode(node), priority);
            }

            // Returns the distance from the node to the Destination node
            float hScore(Node node)
            {
                return 1.0001f * Math.Abs(Destination.Position.X - node.Position.X) + (Destination.Position.Y - node.Position.Y);
            }

            // Add Source to the queue & set Source parent & set Source gScore
            Enqueue(Source, 0);
            parent[Source.Id] = Source;
            gScore[Source.Id] = 0;

            // While there are nodes in the queue
            while (priorityQueue.Any())
            {
                // dequeue the node with the lowest priority
                var u = priorityQueue.Dequeue().Value;
                visitedNodes++;

                // Check if it is the destination node
                if (u.Id == Destination.Id)
                    return ConstructPathFromParents(parent);

                // Add all unvisited neighbors to the queue & assign their parent as the current node if it is unvisited or will give it a lower cost:
                // For each neighbor (v) of the current node (u)
                foreach (var v in u.Adjacents)
                {
                    // newCost is the total cost from the source to the neighbor node.
                    // its value will be the current nodes cost + the edge weight from the current node to the neighbor
                    var newCost = gScore[u.Id] + v.Weight;

                    var VGScore = gScore[v.Id];
                    // If v is unvisited (i.e. v has no assigned parent) or has a higher cost than newCost
                    if (VGScore == null || VGScore > newCost)
                    {
                        parent[v.Id] = u;
                        gScore[v.Id] = newCost;
                        Enqueue(Graph.Nodes[v.Id], gScore[v.Id].Value + hScore(Graph.Nodes[v.Id]));
                    }
                }
            }

            // No solution found
            return null;
        }

        /// <summary>
        /// Constructs a list on node id's from the destination node to the source node. 
        /// </summary>
        /// <param name="parent">Array that contains all necessary nodes parents</param>
        /// <returns>A list on node id's from the destination node to the source node.</returns>
        private List<Node> ConstructPathFromParents(Node[] parent)
        {
            var path = new List<Node>();

            var currentNode = Destination;
            var nextNode = parent[Destination.Id];

            while(currentNode.Id != nextNode.Id)
            {
                path.Add(currentNode);

                currentNode = nextNode;
                nextNode = parent[currentNode.Id];
            }
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
