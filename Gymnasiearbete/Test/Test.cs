using Gymnasiearbete.Pathfinding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using static Gymnasiearbete.Pathfinding.Pathfinder;

namespace Gymnasiearbete.Test
{
    static class Test
    {
        /// <summary>
        /// Tests all path-finding algorithms on all graphs and returns the results.
        /// </summary>
        /// <returns>Test results.</returns>
        public static TestResults RunTests()
        {
            Console.WriteLine("Running test...");

            return new TestResults
            {
                OpennesResults = GetOpennessResults(GraphManager.saveLocation)
            };
        }

        /// <summary>
        /// Tests all graphs within a SavedGraphs directory and returns the results.
        /// </summary>
        /// <param name="pathToDirectorieWithOpennesDirectories"></param>
        /// <returns></returns>
        private static List<OpennessResults> GetOpennessResults(string pathToDirectorieWithOpennesDirectories)
        {
            var opennessResults = new List<OpennessResults>();

            var opennessDirectories = Directory.GetDirectories(pathToDirectorieWithOpennesDirectories);

            foreach (var opennessDirectory in opennessDirectories)
            {
                // get openness
                int.TryParse(Path.GetDirectoryName(opennessDirectory), out int graphOpenness);
 
                opennessResults.Add(new OpennessResults
                {
                    Openness = graphOpenness,
                    SizeResults = GetSizeResults(opennessDirectory),
                });
            }

            return opennessResults;
        }

        /// <summary>
        /// Tests all graphs within an openness directory and returns the results.
        /// </summary>
        /// <param name="pathToDirectorieWithGraphSizeDirectories"></param>
        /// <returns></returns>
        private static List<SizeResults> GetSizeResults(string pathToDirectorieWithGraphSizeDirectories)
        {
            var sizeResults = new List<SizeResults>();

            var sizeDirectories = Directory.GetDirectories(pathToDirectorieWithGraphSizeDirectories);

            // For each size directory in openness directory
            foreach (var sizeDirectory in sizeDirectories)
            {
                // get size
                int.TryParse(Path.GetFileName(sizeDirectory), out int graphSize);

                sizeResults.Add(new SizeResults
                {
                    GraphSize = graphSize,
                    SizeRepeatResults = GetSizeRepeatResults(sizeDirectory),
                });
            }

            return sizeResults;
        }

        /// <summary>
        /// Tests all graphs within a graph size directory and returns the results.
        /// </summary>
        /// <param name="pathToDirectoryWithGraphSizeRepetFiles"></param>
        /// <returns></returns>
        private static List<SizeRepeatResults> GetSizeRepeatResults(string pathToDirectoryWithGraphSizeRepetFiles)
        {
            var sizeRepeatResults = new List<SizeRepeatResults>();

            var graphFilesPaths = Directory.GetFiles(pathToDirectoryWithGraphSizeRepetFiles);

            // For each graph file in size directory
            int index = 0;
            foreach (var graphFilePath in graphFilesPaths)
            {
                // load graph from disk
                var graph = GraphManager.Load(graphFilePath);

                sizeRepeatResults.Add(new SizeRepeatResults
                {
                    GraphSizeRepet = index,
                    SearchTypeResults = GetSearchTypeResults(graph),
                });

                index++;
            }

            return sizeRepeatResults;
        }

        /// <summary>
        /// Tests a graph and returns the results for all searchTypes.
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        private static List<SearchTypeResults> GetSearchTypeResults(Graph graph)
        {
            var searchTypeResults = new List<SearchTypeResults>();

            // create new pathfinder with the source node in the top left corner and destination node in the bottom right corner
            var pathFinder = new Pathfinder(graph, 0, graph.AdjacencyList.Count - 1);

            // Test graph with each searchType
            foreach (SearchType searchType in Enum.GetValues(typeof(SearchType)))
            {
                searchTypeResults.Add(new SearchTypeResults
                {
                    SearchType = searchType,
                    SearchResults = GetSearchResults(pathFinder, searchType)
                });
            }

            return searchTypeResults;
        }

        /// <summary>
        /// Tests a graph with the given searchType multiple times and returns the results.
        /// </summary>
        /// <returns></returns>
        private static List<SearchResult> GetSearchResults(Pathfinder pathfinder, SearchType searchType)
        {
            var searchResults = new List<SearchResult>();

            for (int i = 0; i < 3; i++)
            {
                searchResults.Add(GetSearchResult(pathfinder, searchType));
            }

            return searchResults;
        }

        /// <summary>
        /// Tests a graph with the given searchType and returns the result.
        /// </summary>
        /// <param name="pathfinder"></param>
        /// <param name="searchType"></param>
        /// <returns></returns>
        private static SearchResult GetSearchResult(Pathfinder pathfinder, SearchType searchType)
        {
            var stopwatch = new Stopwatch(); // TODO: don't create new stopwatch each time

            // start timer
            stopwatch.Restart();
            // run pathfinder
            bool foundPath = pathfinder.FindPath(searchType, out var path);
            // stop timer
            stopwatch.Stop();

            // if the path was not found, time will be set to -1
            var elapsedTime = foundPath ? stopwatch.Elapsed.TotalSeconds : -1;

            return new SearchResult
            {
                SearchTime = elapsedTime,
            };
        }
    }
}
