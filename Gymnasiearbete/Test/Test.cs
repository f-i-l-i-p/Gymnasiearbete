using Gymnasiearbete.Pathfinding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static Gymnasiearbete.Pathfinding.Pathfinder;

namespace Gymnasiearbete.Test
{
    static class ResultSetup
    {
        public static TestResult SetupTestResult()
        {
            var testResult = new TestResult
            {
                GraphOptimizationResults = SetupGraphOptimizationResults(),
            };

            return testResult;
        }

        private static List<GraphOptimizationResult> SetupGraphOptimizationResults()
        {
            var graphOptimizationResults = new List<GraphOptimizationResult>();

            foreach (OptimizationType optimizationType in Enum.GetValues(typeof(OptimizationType)))
            {
                graphOptimizationResults.Add(new GraphOptimizationResult
                {
                    OptimizationType = optimizationType,
                    SearchTypeResults = SetupSearchTypeResults(),
                });
            }
            return graphOptimizationResults;
        }

        private static List<SearchTypeResult> SetupSearchTypeResults()
        {
            var searchTypeResults = new List<SearchTypeResult>();

            foreach (SearchType searchType in Enum.GetValues(typeof(SearchType)))
            {
                searchTypeResults.Add(new SearchTypeResult
                {
                    SearchType = searchType,
                    OpennessResults = new List<OpennessResult>(),
                });
            }

            return searchTypeResults;
        }
    }

    static class Test
    {
        static int searchRepeat = 3;

        /// <summary>
        /// Tests all path-finding algorithms on all graphs and returns the result.
        /// </summary>
        /// <returns>The test result.</returns>
        public static TestResult RunTests()
        {
            Console.WriteLine("Running test...");


            var testResult = ResultSetup.SetupTestResult();


            // For each graph openness directory
            var opennessDirectories = Directory.GetDirectories(GraphManager.saveLocation);
            foreach (var opennessDirectory in opennessDirectories)
            {
                // parse openness
                double.TryParse(Path.GetFileName(opennessDirectory), out double graphOpenness);

                // For each graph size directory
                var sizeDirectories = Directory.GetDirectories(opennessDirectory);
                foreach (var sizeDirectory in sizeDirectories)
                {
                    // parse size
                    int.TryParse(Path.GetFileName(sizeDirectory), out int graphSize);

                    // For each graph file
                    var graphFiles = Directory.GetFiles(sizeDirectory);
                    foreach (var graphFile in graphFiles)
                    {
                        // parse repeat
                        int.TryParse(Path.GetFileName(graphFile), out int graphRepeat);

                        // load graph
                        var graph = GraphManager.Load(graphFile);

                        var pathfinder = new Pathfinder(graph, 0, graph.AdjacencyList.Count - 1);

                        // For each SerchTypeResult in current GraphOptimizationResult
                        foreach (SearchTypeResult searchTypeResult in testResult.GraphOptimizationResults[0].SearchTypeResults)
                        {
                            var or = GetOpennessResult(graphOpenness, searchTypeResult);
                            var sr = GetSizeResult(graphSize, or);
                            var srr = GetSizeRepeatResult(graphRepeat, sr);

                            // test pathfinder and save results
                            srr.SearchResults = GetSearchResults(pathfinder, searchTypeResult.SearchType);
                        }
                    }
                }
            }


            return testResult;
        }


        /// <summary>
        /// Returns the OpennessResult with a specific openness.
        /// If the SearchTypeResult does not contain an OpennessResult with the matching openness,
        /// a new OpennessResult with that openness will be added and returned.
        /// </summary>
        /// <param name="openness">Openness to match.</param>
        /// <param name="searchTypeResult">SearchTypeResult containing the OpennessResults.</param>
        /// <returns>The OpennessResult with matching openness.</returns>
        private static OpennessResult GetOpennessResult(double openness, SearchTypeResult searchTypeResult)
        {
            if (searchTypeResult.OpennessResults == null)
                searchTypeResult.OpennessResults = new List<OpennessResult>();
            

            int index = searchTypeResult.OpennessResults.FindIndex(x => x.Openness == openness);

            if (index == -1)
            {
                searchTypeResult.OpennessResults.Add(new OpennessResult{ Openness = openness });
                return searchTypeResult.OpennessResults.Last();
            }
            else
                return searchTypeResult.OpennessResults[index];
        }

        /// <summary>
        /// Returns the SizeResult with s specific size.
        /// If the OpennessResult does not contain a SizeResult with the matching size,
        /// a new SizeReult with that size will be added and returned.
        /// </summary>
        /// <param name="size">Size to match.</param>
        /// <param name="opennessResult">OpennessResult containing the SizeResults.</param>
        /// <returns>The SizeResult with matching size.</returns>
        private static SizeResult GetSizeResult(int size, OpennessResult opennessResult)
        {
            if (opennessResult.SizeResults == null)
                opennessResult.SizeResults = new List<SizeResult>();

            int index = opennessResult.SizeResults.FindIndex(x => x.GraphSize == size);

            if (index == -1)
            {
                opennessResult.SizeResults.Add(new SizeResult
                {
                    GraphSize = size,
                });
                return opennessResult.SizeResults.Last();
            }
            else
                return opennessResult.SizeResults[index];
        }

        /// <summary>
        /// Returns the SizeRepeatResult with a specific openness.
        /// If the SizeResult does no contain a SizeRepeatResult with the matching repeat,
        /// a new SizeRepeatResult will be added and returned.
        /// </summary>
        /// <param name="repeat">Repeat to match.</param>
        /// <param name="sizeResult">SizeResult containing the SizeRepeatResults</param>
        /// <returns>The SizeResult with matching size.</returns>
        private static SizeRepeatResult GetSizeRepeatResult(int repeat, SizeResult sizeResult)
        {
            if (sizeResult.SizeRepeatResults == null)
                sizeResult.SizeRepeatResults = new List<SizeRepeatResult>();

            int index = sizeResult.SizeRepeatResults.FindIndex(x => x.GraphSizeRepet == repeat);

            if (index == -1)
            {
                sizeResult.SizeRepeatResults.Add(new SizeRepeatResult
                {
                    GraphSizeRepet = repeat,
                    SearchResults = new List<SearchResult>(),
                });
                return sizeResult.SizeRepeatResults.Last();
            }
            else
                return sizeResult.SizeRepeatResults[index];
        }


        private static List<GraphOptimizationResult> GetGraphOptimizationResults()
        {
            return new List<GraphOptimizationResult>
            {
                new GraphOptimizationResult
                {
                    OptimizationType = OptimizationType.None,
                    SearchTypeResults = GetSearchTypeResults(OptimizationType.None),
                },
                new GraphOptimizationResult
                {
                    OptimizationType = OptimizationType.Shrinked,
                    SearchTypeResults = GetSearchTypeResults(OptimizationType.Shrinked),
                }
            };
        }

        /// <summary>
        /// Tests all SearchTypes on all graphs with the specified optimizationType
        /// </summary>
        /// <param name="optimizationType">OptimizationType to use</param>
        /// <returns>All SearchTypeResults</returns>
        private static List<SearchTypeResult> GetSearchTypeResults(OptimizationType optimizationType)
        {
            var searchTypeResults = new List<SearchTypeResult>();

            foreach (SearchType searchType in Enum.GetValues(typeof(SearchType)))
            {
                searchTypeResults.Add(new SearchTypeResult
                {
                    SearchType = searchType,
                    OpennessResults = GetOpennessResults(searchType)
                });
            }

            return searchTypeResults;
        }

        private static List<OpennessResult> GetOpennessResults(OptimizationType optimizationType)



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
                double.TryParse(Path.GetFileName(opennessDirectory), out double graphOpenness);
 
                opennessResults.Add(new OpennessResults
                {
                    Openness = graphOpenness,
                    SizeResults = GetSizeResultsList(opennessDirectory),
                });
            }

            return opennessResults;
        }

        /// <summary>
        /// Tests all graphs within an openness directory and returns the results.
        /// </summary>
        /// <param name="pathToDirectorieWithGraphSizeDirectories"></param>
        /// <returns></returns>
        private static List<SizeResults> GetSizeResultsList(string pathToDirectorieWithGraphSizeDirectories)
        {
            var sizeResultsList = new List<SizeResults>();

            var sizeDirectories = Directory.GetDirectories(pathToDirectorieWithGraphSizeDirectories);

            // For each size directory in openness directory
            foreach (var sizeDirectory in sizeDirectories)
            {
                // get size
                int.TryParse(Path.GetFileName(sizeDirectory), out int graphSize);

                var sizeResults = new SizeResults
                {
                    GraphSize = graphSize,
                    SizeRepeatResults = GetSizeRepeatResults(sizeDirectory),
                };

                // set average results, based on SizeRepetResults
                sizeResults.AverageSearchTypeResults = CalculateAvargeSearchTypeResults(sizeResults.SizeRepeatResults);

                sizeResultsList.Add(sizeResults);
            }

            return sizeResultsList;
        }

        /// <summary>
        /// Calculates AverageSearchResult for each SearchType from the data in the sizeRepeatResultsList.
        /// </summary>
        /// <param name="sizeRepeatResultsList">SizeRepeatResults</param>
        /// <returns>AverageSearchTypeResult for all SearchTypes</returns>
        private static List<AverageSearchTypeResult> CalculateAvargeSearchTypeResults(List<SizeRepeatResults> sizeRepeatResultsList)
        {
            var avarageSearchTypeResults = new List<AverageSearchTypeResult>();

            // All SeachTypeResults
            var allSearchTypeResults = new List<SearchTypeResults>();
            // Add SearchTypeResults with an empty SearchResults list
            foreach (SearchType searchType in Enum.GetValues(typeof(SearchType)))
            {
                allSearchTypeResults.Add(new SearchTypeResults
                {
                    SearchType = searchType,
                    SearchResults = new List<SearchResult>(),
                });
            }

            // Add all searchResults to allSearchTypeResults
            // For each sizeRepeatResults
            foreach (var sizeRepeatResults in sizeRepeatResultsList)
            {
                // For each seachTypeResults
                foreach (var searchTypeResults in sizeRepeatResults.SearchTypeResults)
                {
                    // find matching searchType in allSearchTypeResults, and add the results from this searchTypeResults
                    allSearchTypeResults.Find(x => x.SearchType == searchTypeResults.SearchType).SearchResults.AddRange(searchTypeResults.SearchResults);
                }
            }

            // Calculate average
            foreach (var searchTypeResults in allSearchTypeResults)
            {
                var results = searchTypeResults.SearchResults;

                // sort results based on search time
                results.Sort((x, y) => x.SearchTime.CompareTo(y.SearchTime));

                // calculate median
                var median = results.Count % 2 == 0 ? results[results.Count / 2 - 1] : results[results.Count / 2];
                // calculate mean
                var meanTime = results.Sum(x => x.SearchTime) / results.Count;
                var mean = new SearchResult { SearchTime = meanTime, ExplordedNodes = results[0].ExplordedNodes, ExploredRatio = results[0].ExploredRatio };

                avarageSearchTypeResults.Add(new AverageSearchTypeResult
                {
                    SearchType = searchTypeResults.SearchType,
                    MedianSearchResult = median,
                    MeanSearchResult = mean,
                });
            }

            return avarageSearchTypeResults;
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

            for (int i = 0; i < searchRepeat; i++)
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
