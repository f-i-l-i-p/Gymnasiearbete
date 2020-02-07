using Gymnasiearbete.Graphs;
using System.Collections.Generic;
using System.IO;
using static Gymnasiearbete.Pathfinding.GraphPruning;
using static Gymnasiearbete.Pathfinding.Pathfinder;

namespace Gymnasiearbete.Test
{
    static class TestManager
    {
        /// <summary>
        /// Runs tests on all graphs and saves the result.
        /// </summary>
        public static void RunTests(IEnumerable<PruningAlgorithm> pruningAlgorithms, IEnumerable<PathfindingAlgorithm> pathfindingAlgorithms, int searchRepeat)
        {
            var results = Test.RunTests(pruningAlgorithms, pathfindingAlgorithms, searchRepeat);

            SaveResult(results);
        }

        /// <summary>
        /// Saves test results
        /// </summary>
        /// <param name="results">Results to be saved.</param>
        private static void SaveResult(TestResult results)
        {
            SaveAsJson(results);
            SaveAsExcel(results);
        }

        /// <summary>
        /// Saves test results as an excel file.
        /// </summary>
        /// <param name="results">Test results to be saved.</param>
        private static void SaveAsExcel(TestResult results)
        {
            var excel = GenerateExcel.Generate(results);

            // save
            excel.Save(GraphManager.saveLocation, "results");
        }

        /// <summary>
        /// Saves test results as a json file.
        /// </summary>
        /// <param name="results">Test results to be saved.</param>
        private static void SaveAsJson(TestResult results)
        {
            // convert results to json
            var contents = Newtonsoft.Json.JsonConvert.SerializeObject(results);
            // save
            File.WriteAllText($"{GraphManager.saveLocation}/results.json", contents);
        }
    }
}
