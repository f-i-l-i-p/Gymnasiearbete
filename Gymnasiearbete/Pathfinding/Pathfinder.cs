﻿using System;
using System.Collections.Generic;
using System.Linq;
using Gymnasiearbete.Graphs;
using Gymnasiearbete.Pathfinding.QueueNodes;
using Priority_Queue;

namespace Gymnasiearbete.Pathfinding
{
    public class Pathfinder
    {
        /// <summary>
        /// All path-finding algorithms that are implemented.
        /// </summary>
        public enum PathfindingAlgorithm { BFS, Dijkstras, AStar };

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
        /// Searches the graph with the specified path-finding algorithm to find the shortest path from the source node to the destination node.
        /// </summary>
        /// <param name="pathfindingAlgorithm">Path-finding algorithm to use.</param>
        /// <returns>A list of steps to take to travel from the source node to the destination node.
        /// If no path is found, null is returned.</returns>
        public List<Node> FindPath(PathfindingAlgorithm pathfindingAlgorithm, out int visitedNodes)
        {
            switch (pathfindingAlgorithm)
            {
                case PathfindingAlgorithm.BFS:
                    return BFS(out visitedNodes);
                case PathfindingAlgorithm.Dijkstras:
                    return Dijkstras(out visitedNodes);
                case PathfindingAlgorithm.AStar:
                    return AStar(out visitedNodes);
                default:
                    throw new NotImplementedException($"{pathfindingAlgorithm.ToString()} is not implemented");
            }
        }

        /// <summary>
        /// Searches the graph with the Breadth First Search algorithm to find the shortest path from the source node to the destination node.
        /// </summary>
        /// <param name="visitedNodes">The total number of nodes visited.</param>
        /// <returns>A list of steps to take to travel from the source node to the destination node.
        /// If no path is found, null is returned.</returns>
        public List<Node> BFS(out int visitedNodes)
        {
            var queue = new Queue<Node>();
            var parent = new Node[Graph.Nodes.Count];

            // Add Source as a start node
            queue.Enqueue(Source);
            parent[Source.Id] = Source;

            // While there are nodes to check
            while (queue.Count > 0)
            {
                // dequeue the first node in the queue
                var current = queue.Dequeue();

                // Check if it is the destination node
                if (current.Id == Destination.Id)
                {
                    visitedNodes = CountVisitedNodes(parent);
                    return ConstructPathFromParents(parent);
                }

                // Loop through all neighbors
                foreach (var adjacent in current.Adjacents)
                {
                    // If this adjacent node is unvisited (i.e. it has no assigned parent)
                    if (parent[adjacent.Id] == null)
                    {
                        parent[adjacent.Id] = current;
                        queue.Enqueue(Graph.Nodes[adjacent.Id]);
                    }
                }
            }

            // No solution found
            visitedNodes = CountVisitedNodes(parent);
            return null;
        }

        /// <summary>
        /// Searches the graph with Dijkstra's algorithm to find the shortest path from the source node to the destination node.
        /// </summary>
        /// <param name="visitedNodes">The total number of nodes visited.</param>
        /// <returns>A list of steps to take to travel from the source node to the destination node.
        /// If no path is found, null is returned.</returns>
        public List<Node> Dijkstras(out int visitedNodes)
        {
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
                var current = priorityQueue.Dequeue().Value;

                // Check if it is the destination node
                if (current == Destination)
                {
                    visitedNodes = CountVisitedNodes(parent);
                    return ConstructPathFromParents(parent);
                }

                // Loop through all neighbors
                foreach (var adjacent in current.Adjacents)
                {
                    // gNew is the total cost to travel the current path from Source to this adjacent node
                    var gNew = gScore[current.Id] + adjacent.Weight;

                    // If this adjacent node is unvisited (i.e. it has no assigned g-score) or already has a g-score higher than the new g-score
                    if (gScore[adjacent.Id] == null || gScore[adjacent.Id] > gNew)
                    {
                        gScore[adjacent.Id] = gNew;
                        parent[adjacent.Id] = current;
                        Enqueue(Graph.Nodes[adjacent.Id], gNew.Value);
                    }
                }
            }

            // No solution found
            visitedNodes = CountVisitedNodes(parent);
            return null;
        }

        /// <summary>
        /// Searches the graph with the A* algorithm to find the shortest path from the source node to the destination node.
        /// </summary>
        /// <param name="visitedNodes">The total number of nodes visited.</param>
        /// <returns>A list of steps to take to travel from the source node to the destination node.
        /// If no path is found, null is returned.</returns>
        public List<Node> AStar(out int visitedNodes)
        {
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

            // Returns the h-score for a node
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
                var current = priorityQueue.Dequeue().Value;

                // Check if it is the destination node
                if (current.Id == Destination.Id)
                {
                    visitedNodes = CountVisitedNodes(parent);
                    return ConstructPathFromParents(parent);
                }

                // Loop trough all neighbors
                foreach (var adjacent in current.Adjacents)
                {
                    // gNew is the total cost to travel the current path from Source to this adjacent node
                    var gNew = gScore[current.Id] + adjacent.Weight;

                    // If this adjacent node is unvisited (i.e. it has no assigned g-score) or already has a g-score higher than the new g-score
                    if (gScore[adjacent.Id] == null || gScore[adjacent.Id] > gNew)
                    {
                        gScore[adjacent.Id] = gNew;
                        parent[adjacent.Id] = current;
                        Enqueue(Graph.Nodes[adjacent.Id], gScore[adjacent.Id].Value + hScore(Graph.Nodes[adjacent.Id]));
                    }
                }
            }

            // No solution found
            visitedNodes = CountVisitedNodes(parent);
            return null;
        }

        /// <summary>
        /// Constructs a list on node id's from the destination node to the source node. 
        /// </summary>
        /// <param name="parents">Array that contains all necessary nodes parents.</param>
        /// <returns>A list on node id's from the destination node to the source node.</returns>
        private List<Node> ConstructPathFromParents(Node[] parents)
        {
            Node currentNode = Destination;
            Node nextNode;

            var path = new List<Node> { currentNode };

            while(currentNode.Id != (nextNode = parents[currentNode.Id]).Id)
            {
                currentNode = nextNode;
                path.Add(currentNode);
            }
            return path;
        }

        /// <summary>
        /// Counts all visited nodes from array of parents.
        /// </summary>
        /// <param name="parents">Array that contain all visited nodes parents.</param>
        /// <returns>Number of visited nodes.</returns>
        private int CountVisitedNodes(Node[] parents)
        {
            int n = 0;

            foreach (var parent in parents)
            {
                if (parent != null)
                    n++;
            }
            return n;
        }
    }
}
