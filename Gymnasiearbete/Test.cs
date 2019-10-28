using Gymnasiearbete.Pathfinding;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Gymnasiearbete.Pathfinding.Pathfinder;

namespace Gymnasiearbete
{
    static class Test
    {
        /// <summary>
        /// Tests all path-finding algorithms on all graphs and saves the results to a .xlsx file
        /// </summary>
        public static void Run()
        {
            // object to save test data
            var testResult = new TestResult
            {
                GraphResults = new List<GraphResult>(),
            };

            List<string> graphPaths = GraphManager.GetAllGraphPaths();

            foreach (var path in graphPaths)
            {
                // load graph from disk
                var graph = GraphManager.Load(path);

                testResult.GraphResults.Add(RunGraphPathfinderTests(graph, path.Split('\\').Last()));
            }

            SaveResult(testResult);
        }

        // TODO: save result
        private static void SaveResult(TestResult testResult)
        {
            // Create excel with one empty sheet
            var excel = new Excel();
            excel.AddSheet();

            int currentRow = 0;

            foreach (var graphResult in testResult.GraphResults)
            {
                // set graph size
                excel.SetCell(0, currentRow, graphResult.GraphSize, new SimpleCellStyle { BorderBottom = BorderStyle.Thick });

                // TODO: set graph count

                foreach (var searchTypeResult in graphResult.SearchTypesResults)
                {
                    // set search type
                    excel.SetCell(2, currentRow, searchTypeResult.SearchType.ToString());

                    var SearchTimes = new List<double>();
                    var ExplordedNodes = new List<int>();
                    var ExplordedRatios = new List<double>();
                    foreach (var item in searchTypeResult.SearchResults)
                    {
                        SearchTimes.Add(item.SearchTime);
                        ExplordedNodes.Add(item.ExplordedNoedes);
                        ExplordedRatios.Add(item.ExploredRatio);
                    }
                    // set search times
                    excel.SetCell(3, currentRow, "Search time");
                    excel.SetRow(4, currentRow, SearchTimes);
                    currentRow++;
                    // set explored nodes
                    excel.SetCell(3, currentRow, "Explored nodes");
                    excel.SetRow(4, currentRow, ExplordedNodes);
                    currentRow++;
                    // set explored ratio
                    excel.SetCell(3, currentRow, "Explored ratio");
                    excel.SetRow(4, currentRow, ExplordedRatios);
                    currentRow++;
                }
            }

            excel.SelectedSheet.AutoSizeColumn(3);

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
                GraphSize = graph.AdjacencyList.Count,
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

    class TestResult
    {
        public List<GraphResult> GraphResults { get; set; }
    }

    class GraphResult
    {
        public string GraphName {get; set;}
        public int GraphSize { get; set; }
        public List<SearchTypeResult> SearchTypesResults { get; set; }
    }

    class SearchTypeResult
    {
        public SearchType SearchType { get; set; }
        public List<SearchResult> SearchResults { get; set; }
    }

    class SearchResult
    {
        public double SearchTime { get; set; }
        public int ExplordedNoedes { get; set; }
        public double ExploredRatio { get; set; }
    }
}