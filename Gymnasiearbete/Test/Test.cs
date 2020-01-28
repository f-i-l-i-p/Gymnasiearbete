using Gymnasiearbete.Graphs;
using Gymnasiearbete.Pathfinding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static Gymnasiearbete.Pathfinding.GraphOptimization;
using static Gymnasiearbete.Pathfinding.Pathfinder;

namespace Gymnasiearbete.Test
{
    static class ResultSetup
    {
        public static TestResult SetupTestResult(IEnumerable<OptimizationType> optimizationTypes, IEnumerable<SearchType> searchTypes)
        {
            var testResult = new TestResult
            {
                GraphOptimizationResults = SetupGraphOptimizationResults(optimizationTypes, searchTypes),
            };

            return testResult;
        }

        private static List<GraphOptimizationResult> SetupGraphOptimizationResults(IEnumerable<OptimizationType> optimizationTypes, IEnumerable<SearchType> searchTypes)
        {
            var graphOptimizationResults = new List<GraphOptimizationResult>();

            foreach (var optimizationType in optimizationTypes)
            {
                graphOptimizationResults.Add(new GraphOptimizationResult
                {
                    OptimizationType = optimizationType,
                    SearchTypeResults = SetupSearchTypeResults(searchTypes),
                });
            }
            return graphOptimizationResults;
        }

        private static List<SearchTypeResult> SetupSearchTypeResults(IEnumerable<SearchType> searchTypes)
        {
            var searchTypeResults = new List<SearchTypeResult>();

            foreach (SearchType searchType in searchTypes)
            {
                searchTypeResults.Add(new SearchTypeResult
                {
                    SearchType = searchType,
                    ComplexityResults = new List<ComplexityResult>(),
                });
            }

            return searchTypeResults;
        }
    }

    static class Test
    {
        private static int searchRepeat;

        /// <summary>
        /// Tests all path-finding algorithms on all graphs and returns the result.
        /// </summary>
        /// <returns>The test result.</returns>
        public static TestResult RunTests(IEnumerable<OptimizationType> optimizationTypes, IEnumerable<SearchType> searchTypes, int _searchRepeat)
        {
            searchRepeat = _searchRepeat;

            // setup an empty test result
            var testResult = ResultSetup.SetupTestResult(optimizationTypes, searchTypes);

            var optimizationTimer = new Stopwatch();

            var totalSearchCount = Directory.GetFiles(GraphManager.saveLocation, "*.json", SearchOption.AllDirectories).Length;
            int searchCount = 0;

            // For each graph complexity directory
            var complexityDirectories = Directory.GetDirectories(GraphManager.saveLocation);
            int ODIndex = 0;
            foreach (var complexityDirectory in complexityDirectories)
            {
                // parse complexity
                double.TryParse(Path.GetFileName(complexityDirectory), out double graphComplexity);

                // For each graph size directory
                var sizeDirectories = SortByGraphSize(Directory.GetDirectories(complexityDirectory));
                foreach (var sizeDirectory in sizeDirectories)
                {
                    // print progress
                    Console.WriteLine($"{string.Format("{0:0.00}", 100 * (double)searchCount / totalSearchCount)}%");

                    // parse size
                    int.TryParse(Path.GetFileName(sizeDirectory), out int graphSize);

                    // For each graph file
                    var graphFiles = Directory.GetFiles(sizeDirectory);
                    foreach (var graphFile in graphFiles)
                    {
                        // parse repeat
                        int.TryParse(Path.GetFileNameWithoutExtension(graphFile), out int graphRepeat);

                        // For each GraphOptimizationResult
                        foreach (var graphOptimizationResult in testResult.GraphOptimizationResults)
                        {
                            // load graph
                            var graph = GraphManager.Load(graphFile);

                            var sourceNode = graph.Nodes[0];
                            var destinationNode = graph.Nodes[graph.Nodes.Count - 1];

                            double optimizationTime;

                            // Optimize graph
                            switch (graphOptimizationResult.OptimizationType)
                            {
                                case OptimizationType.None:
                                    optimizationTime = 0;
                                    break;
                                case OptimizationType.IntersectionJumps:
                                    optimizationTimer.Restart();
                                    GraphOptimization.IntersectionJumps(graph, new Node[] { sourceNode, destinationNode });
                                    optimizationTimer.Stop();
                                    optimizationTime = optimizationTimer.Elapsed.TotalSeconds;
                                    break;
                                case OptimizationType.CornerJumps:
                                    optimizationTimer.Restart();
                                    GraphOptimization.CornerJumps(graph, new Node[] { sourceNode, destinationNode });
                                    optimizationTimer.Stop();
                                    optimizationTime = optimizationTimer.Elapsed.TotalSeconds;
                                    break;
                                default:
                                    throw new NotImplementedException($"{graphOptimizationResult.OptimizationType.ToString()} is not implemented");
                            }

                            var pathfinder = new Pathfinder(graph, sourceNode, destinationNode);

                            // For each SerchTypeResult in current GraphOptimizationResult
                            foreach (SearchTypeResult searchTypeResult in graphOptimizationResult.SearchTypeResults)
                            {
                                // Find the SizeRepeatResult for the current graph in this searchTypeResult
                                var complexityResult = GetComplexityResult(graphComplexity, searchTypeResult);
                                var sizeResult = GetSizeResult(graphSize, complexityResult);
                                var sizeRepeatResult = GetSizeRepeatResult(graphRepeat, sizeResult);

                                // test pathfinder and save results
                                sizeRepeatResult.SearchResults = GetSearchResults(pathfinder, searchTypeResult.SearchType);
                                // set optimization time
                                sizeRepeatResult.GraphOptimizationTime = optimizationTime;
                                
                                // set average result 
                                sizeResult.AverageSearchResult = CalculateAverageSearchResult(sizeResult.SizeRepeatResults, out var agot);
                                sizeResult.AverageGraphOpimizationTime = agot;
                            }
                        }
                        searchCount++;
                    }
                }
                ODIndex++;
            }


            return testResult;
        }

        /// <summary>
        /// Sorts an array of paths to graph size directories by graph size.
        /// </summary>
        /// <param name="graphDirectories">Array to sort.</param>
        /// <returns>Sorted array.</returns>
        private static string[] SortByGraphSize(string[] graphDirectories)
        {
            return graphDirectories.OrderBy(x => ParseInt(x.Split(new string[] { "\\" }, StringSplitOptions.None).Last())).ToArray();
        }

        public static int ParseInt(string s)
        {
            int.TryParse(s, out int result);
            return result;
        }


        /// <summary>
        /// Returns the ComplexityResult with a specific complexity.
        /// If the SearchTypeResult does not contain a ComplexityResult with the matching complexity,
        /// a new ComplexityResult with that complexity will be added and returned.
        /// </summary>
        /// <param name="complexity">Complexity to match.</param>
        /// <param name="searchTypeResult">SearchTypeResult containing the ComplexityResults.</param>
        /// <returns>The ComplexityResult with matching complexity.</returns>
        private static ComplexityResult GetComplexityResult(double complexity, SearchTypeResult searchTypeResult)
        {
            if (searchTypeResult.ComplexityResults == null)
                searchTypeResult.ComplexityResults = new List<ComplexityResult>();
            

            int index = searchTypeResult.ComplexityResults.FindIndex(x => x.Complexity == complexity);

            if (index == -1)
            {
                searchTypeResult.ComplexityResults.Add(new ComplexityResult{ Complexity = complexity });
                return searchTypeResult.ComplexityResults.Last();
            }
            else
                return searchTypeResult.ComplexityResults[index];
        }

        /// <summary>
        /// Returns the SizeResult with s specific size.
        /// If the ComplexityResult does not contain a SizeResult with the matching size,
        /// a new SizeReult with that size will be added and returned.
        /// </summary>
        /// <param name="size">Size to match.</param>
        /// <param name="complexityResult">ComplexityResult containing the SizeResults.</param>
        /// <returns>The SizeResult with matching size.</returns>
        private static SizeResult GetSizeResult(int size, ComplexityResult complexityResult)
        {
            if (complexityResult.SizeResults == null)
                complexityResult.SizeResults = new List<SizeResult>();

            int index = complexityResult.SizeResults.FindIndex(x => x.GraphSize == size);

            if (index == -1)
            {
                complexityResult.SizeResults.Add(new SizeResult
                {
                    GraphSize = size,
                });
                return complexityResult.SizeResults.Last();
            }
            else
                return complexityResult.SizeResults[index];
        }

        /// <summary>
        /// Returns the SizeRepeatResult with a specific complexity.
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

        /// <summary>
        /// Returns the AverageSearchResult from a list of SizeRepeatResults. out AverageGraphOpimizationTime.
        /// </summary>
        /// <param name="sizeRepeatResults">SizeRepeatResults to calculate the AverageSearchResult from.</param>
        /// <param name="averageGraphOpimizationTime">The AverageGraphOpimizationTime.</param>
        /// <returns>The AverageSearchResult.</returns>
        private static AverageSearchResult CalculateAverageSearchResult(List<SizeRepeatResult> sizeRepeatResults, out AverageGraphOpimizationTime averageGraphOpimizationTime)
        {
            // the sum of all search results
            var searchResSum = new SearchResult
            {
                SearchTime = 0,
                ExplordedNodes = 0,
                ExploredRatio = 0,
            };
            double graphOptimizationTimeSum = 0;

            var allSearchTimes = new List<double>();
            var allExploredNodes = new List<int>();
            var allExploredRatios = new List<double>();
            var allGraphOpimizationTimes = new List<double>();

            // the total amount of search results
            var searchResCount = 0;
            
            // Set searchResSum and searchResCount
            foreach (var sizeRepeatResult in sizeRepeatResults)
            {
                allGraphOpimizationTimes.Add(sizeRepeatResult.GraphOptimizationTime);
                graphOptimizationTimeSum += sizeRepeatResult.GraphOptimizationTime;

                foreach (var searchResult in sizeRepeatResult.SearchResults)
                {
                    searchResSum.SearchTime += searchResult.SearchTime;
                    searchResSum.ExplordedNodes += searchResult.ExplordedNodes;
                    searchResSum.ExploredRatio += searchResult.ExploredRatio;

                    AddSorted(allSearchTimes, searchResult.SearchTime);
                    AddSorted(allExploredNodes, searchResult.ExplordedNodes);
                    AddSorted(allExploredRatios, searchResult.ExploredRatio);
                }
                searchResCount += sizeRepeatResult.SearchResults.Count;
            }

            averageGraphOpimizationTime = new AverageGraphOpimizationTime
            {
                MeanOptimizationTime = graphOptimizationTimeSum / sizeRepeatResults.Count,
                MedianOpimizatonTime = GetCenterValue(allGraphOpimizationTimes),
            };

            return new AverageSearchResult
            {
                MeanSearchResult = new SearchResult
                {
                    SearchTime = searchResSum.SearchTime / searchResCount,
                    ExplordedNodes = searchResSum.ExplordedNodes / searchResCount,
                    ExploredRatio = searchResSum.ExploredRatio / searchResCount,
                },
                MedianSearchResult = new SearchResult
                {
                    SearchTime = GetCenterValue(allSearchTimes),
                    ExplordedNodes = GetCenterValue(allExploredNodes),
                    ExploredRatio = GetCenterValue(allExploredRatios),
                },
            };
        }

        /// <summary>
        /// Inserts a value at its sorted position.
        /// </summary>
        /// <param name="list">List to insert value in.</param>
        /// <param name="value">Value to insert.</param>
        public static void AddSorted<T>(this List<T> list, T value)
        {
            int x = list.BinarySearch(value);
            list.Insert((x >= 0) ? x : ~x, value);
        }

        /// <summary>
        /// Returns the value in the center of the list.
        /// If the list has an uneven length, the average between the two values in the center will be returned.
        /// </summary>
        /// <param name="list">List with values.</param>
        /// <returns>The center value in the list.</returns>
        public static T GetCenterValue<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
                return default;
            if (list.Count == 1)
                return list[0];
            else
                return list.Count % 2 == 0
                    ? (((dynamic)list[list.Count / 2 - 1] + list[list.Count / 2])/2)
                    : list[list.Count / 2];
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
            var path = pathfinder.FindPath(searchType, out var visitedNodes);
            // stop timer
            stopwatch.Stop();

            // if the path was not found, time will be set to -1
            var elapsedTime = path == null ? -1 : stopwatch.Elapsed.TotalSeconds;

            return new SearchResult
            {
                SearchTime = elapsedTime,
                ExplordedNodes = visitedNodes,
                ExploredRatio = (double)visitedNodes / pathfinder.Graph.Nodes.Count,
            };
        }
    }
}
