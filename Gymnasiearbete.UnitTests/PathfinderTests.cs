using System;
using System.Collections.Generic;
using System.IO;
using Gymnasiearbete.Graphs;
using Gymnasiearbete.Pathfinding;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gymnasiearbete.UnitTests
{
    [TestClass]
    public class PathfinderTests
    {
        private readonly string testGraphsPath = Path.GetFullPath(@"..\..\TestGraphs");
        private readonly List<Graph> testGraphs;

        Random rnd = new Random();

        public PathfinderTests()
        {
            rnd = new Random();

            testGraphs = LoadTestGraphs();
        }

        private List<Graph> LoadTestGraphs()
        {
            var testGraphs = new List<Graph>();

            var paths = Directory.GetFiles(testGraphsPath, "*.graph", SearchOption.AllDirectories);
            foreach (var path in paths)
            {
                testGraphs.Add(GraphManager.Load(path));
            }

            return testGraphs;
        }

        private void IsPathApproved(List<Node> path, Graph maze, Node source, Node destination)
        {
            Assert.IsTrue(path[0].Id == destination.Id, "Path does not start with the destination node.");
            Assert.IsTrue(path[path.Count - 1].Id == source.Id, "Path does not end with source node.");

            for (int i = 1; i < path.Count - 1; i++)
            {
                // Check that this node is adjacent with the node before and after in the list
                var node = path[i];
                
                var prevNode = path[i - 1];
                var nextNode = path[i + 1];

                var adjacents = maze.Nodes[node.Id].Adjacents;
                
                Assert.IsTrue(adjacents.Exists(adj => adj.Id == prevNode.Id) && adjacents.Exists(adj => adj.Id == nextNode.Id),
                    "One or more neighboring nodes in the path, are not adjacent in the maze.");
            }
        }

        [TestMethod]
        public void BFS_TestMazes_ReturnPossiblePath()
        {
            foreach (var graph in testGraphs)
            {
                var source = graph.Nodes[rnd.Next(0, graph.Nodes.Count)];
                var destination = graph.Nodes[rnd.Next(0, graph.Nodes.Count)];

                var path = new Pathfinder(graph, source, destination).BFS(out var visitedNodes);

                IsPathApproved(path, graph, source, destination);
            }
        }

        [TestMethod]
        public void Dijkstras_TestMazes_ReturnPossiblePath()
        {
            foreach (var graph in testGraphs)
            {
                var source = graph.Nodes[rnd.Next(0, graph.Nodes.Count)];
                var destination = graph.Nodes[rnd.Next(0, graph.Nodes.Count)];

                var path = new Pathfinder(graph, source, destination).Dijkstras(out var visitedNodes);

                IsPathApproved(path, graph, source, destination);
            }
        }

        [TestMethod]
        public void AStar_TestMazes_ReturnPossiblePath()
        {
            foreach (var graph in testGraphs)
            {
                var source = graph.Nodes[rnd.Next(0, graph.Nodes.Count)];
                var destination = graph.Nodes[rnd.Next(0, graph.Nodes.Count)];

                var path = new Pathfinder(graph, source, destination).AStar(out var visitedNodes);

                IsPathApproved(path, graph, source, destination);
            }
        }

        [TestMethod]
        public void PathLengthComparison()
        {
            foreach (var graph in testGraphs)
            {
                var source = graph.Nodes[rnd.Next(0, graph.Nodes.Count)];
                var destination = graph.Nodes[rnd.Next(0, graph.Nodes.Count)];
                var pathfinder = new Pathfinder(graph, source, destination);

                var pathBFS = pathfinder.BFS(out var visitedNodes);
                var pathDijkstras = pathfinder.Dijkstras(out visitedNodes);
                var pathAStar = pathfinder.AStar(out visitedNodes);

                Assert.IsTrue(pathBFS.Count == pathDijkstras.Count, "BFS and Dijkstra found different length paths");
                Assert.IsTrue(pathBFS.Count == pathAStar.Count, "BFS and Dijkstra found different length paths");
                Assert.IsTrue(pathDijkstras.Count == pathAStar.Count, "BFS and Dijkstra found different length paths");
            }
        }

    }
}
