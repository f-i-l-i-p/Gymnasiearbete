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

            // Add Source as a start node
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

            // no solution found
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
            var gScore = new float?[Graph.Nodes.Count];

            // Adds a node to the priorityQueue
            void Enqueue(Node node, float priority)
            {
                priorityQueue.Enqueue(new FastQueueNode(node), priority);
            }

            // Add Source as a start node
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
                if (u == Destination)
                    return ConstructPathFromParents(parent);

                // Add all unvisited neighbors to the queue, assign their parent, and set their gScore, if it is unvisited or will give it a lower cost:
                // For each neighbor (v) of the current node (u)
                foreach (var v in u.Adjacents)
                {
                    // gNew is the total cost to travel the current path from Source, through u, to v.
                    var gNew = gScore[u.Id] + v.Weight;

                    // If v is unvisited (i.e. v has no assigned cost) or has a higher gScore than the new g score
                    if (gScore[v.Id] == null || gScore[v.Id] > gNew)
                    {
                        gScore[v.Id] = gNew;
                        parent[v.Id] = u;
                        Enqueue(Graph.Nodes[v.Id], gNew.Value);
                    }
                }
            }

            // no solution found
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

            // factor for hScore
            float hScale = 1f / Graph.Nodes.Count + 1;

            // Returns the distance from a node to the Destination node
            float hScore(Node node)
            {
                return hScale * (Math.Abs(Destination.Position.X - node.Position.X) + Math.Abs(Destination.Position.Y - node.Position.Y));
            }

            // Add Source as a start node
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

                // Add all unvisited neighbors to the queue, assign their parent, and set their gScore, if it is unvisited or will give it a lower cost:
                // For each neighbor (v) of the current node (u)
                foreach (var v in u.Adjacents)
                {
                    // gNew is the total cost to travel the current path from Source, through u, to v.
                    var gNew = gScore[u.Id] + v.Weight;

                    // If v is unvisited (i.e. v has no assigned cost) or has a higher cost than the new g score 
                    if (gScore[v.Id] == null || gScore[v.Id] > gNew)
                    {
                        gScore[v.Id] = gNew;
                        parent[v.Id] = u;
                        Enqueue(Graph.Nodes[v.Id], gScore[v.Id].Value + hScore(Graph.Nodes[v.Id]));
                    }
                }
            }

            // no solution found
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

            Node currentNode = Destination;
            Node nextNode;

            while(currentNode.Id != (nextNode = parent[Destination.Id]).Id)
            {
                path.Add(currentNode);

                currentNode = nextNode;
            }
            return path;
        }
    }
}
