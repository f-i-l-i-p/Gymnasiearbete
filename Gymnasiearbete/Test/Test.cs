﻿using Gymnasiearbete.Pathfinding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Gymnasiearbete.Pathfinding.Pathfinder;

namespace Gymnasiearbete.Test
{
    static class Test
    {
        /// <summary>
        /// Tests all path-finding algorithms on all graphs and saves the results to a .xlsx file
        /// </summary>
        public static void Run()
        {
            Console.WriteLine("Running test...");


            var testResults = new List<TestResult>();

            var allGraphPaths = GraphManager.GetAllPerfectGraphPaths();

            // For each graph size
            foreach (var graphPaths in allGraphPaths)
            {
                if (graphPaths.Length == 0)
                    continue;


                string sizeString = graphPaths[0].Split('\\').ElementAt(graphPaths[0].Split('\\').Length - 2);
                if (!int.TryParse(sizeString, out int graphSize))
                    continue;

                var testResult = new TestResult
                {
                    GraphSize = graphSize,
                    GraphResults = new List<GraphResult>(),
                };

                // For each graph
                foreach (var path in graphPaths)
                {
                    // load graph from disk
                    var graph = GraphManager.Load(path);

                    // RunGrapPathfinderTests & save the result to testResult.GraphResults
                    testResult.GraphResults.Add(RunGraphPathfinderTests(graph, path.Split('\\').Last()));
                }

                testResults.Add(testResult);
            }

            SaveResult(testResults);
        }

        // TODO: save result
        private static void SaveResult(List<TestResult> testResults)
        {
            Console.WriteLine("Creating excel...");

            var excel = GenerateExcel.Generate(testResults);

            // Save
            excel.Save(GraphManager.saveLocation, "TestFile");
        }

        /// <summary>
        /// Tests all path-finding algorithms on a specified graph.
        /// </summary>
        /// <param name="graph">Graph for testing.</param>
        /// <param name="graphName">Name of graph.</param>
        /// <returns>The test result.</returns>
        private static GraphResult RunGraphPathfinderTests(Graph graph, string graphName)
        {
            // data to return
            var graphResult = new GraphResult
            {
                GraphName = graphName,
                SearchTypesResults = new List<SearchTypeResult>(),
            };

            // Test graph with all pathfinders
            foreach (SearchType searchType in Enum.GetValues(typeof(SearchType)))
            {
                // test path-finding algorithm and save the result 
                graphResult.SearchTypesResults.Add(TestPathfinder(searchType, graph, 3));
            }

            return graphResult;
        }

        /// <summary>
        /// Tests a specified path-finding algorithm and measures its execution time.
        /// </summary>
        /// <param name="searchType">Algorithm to test.</param>
        /// <param name="graph">Graph for testing.</param>
        /// <param name="repet">Number of times the test should be conducted.</param>
        /// <returns>The test result.</returns>
        private static SearchTypeResult TestPathfinder(SearchType searchType, Graph graph, int repet)
        {
            // data to return
            var searchTypeResult = new SearchTypeResult
            {
                SearchType = searchType,
                SearchResults = new List<SearchResult>()
            };

            // create pathfinder
            var pathfinder = new Pathfinder(graph, 0, graph.AdjacencyList.Count - 1);

            var stopwatch = new Stopwatch();
            // Run tests
            for (int i = 0; i < repet; i++)
            {
                // start timer
                stopwatch.Restart();
                // run pathfinder
                var foundPath = pathfinder.FindPath(searchType, out var path);
                // stop timer
                stopwatch.Stop();

                // if the path was not found, time will be set to -1
                var elapsedTime = foundPath ? stopwatch.Elapsed.TotalSeconds : -1;
                searchTypeResult.SearchResults.Add(new SearchResult
                {
                    SearchTime = elapsedTime,
                    //TODO: ExploredNodes, ExploredRatio
                });
            }

            return searchTypeResult;
        }
    }
}