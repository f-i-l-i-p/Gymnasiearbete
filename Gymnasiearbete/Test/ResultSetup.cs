using System.Collections.Generic;
using static Gymnasiearbete.Pathfinding.GraphPruning;
using static Gymnasiearbete.Pathfinding.Pathfinder;

namespace Gymnasiearbete.Test
{
    static class ResultSetup
    {
        public static TestResult SetupTestResult(IEnumerable<PruningAlgorithm> pruningAlgorithms, IEnumerable<PathfindingAlgorithm> pathfindingAlgorithms)
        {
            var testResult = new TestResult
            {
                GraphPruningResults = SetupGraphPruningResults(pruningAlgorithms, pathfindingAlgorithms),
            };

            return testResult;
        }

        private static List<GraphPruningResult> SetupGraphPruningResults(IEnumerable<PruningAlgorithm> pruningAlgorithms, IEnumerable<PathfindingAlgorithm> pathfindingAlgorithms)
        {
            var graphPruningResults = new List<GraphPruningResult>();

            foreach (var pruningAlgorithm in pruningAlgorithms)
            {
                graphPruningResults.Add(new GraphPruningResult
                {
                    PruningAlgorithm = pruningAlgorithm,
                    PathfindingAlgorithmResults = SetupPathfindingAlgorithmResults(pathfindingAlgorithms),
                });
            }
            return graphPruningResults;
        }

        private static List<PathfindingAlgorithmResult> SetupPathfindingAlgorithmResults(IEnumerable<PathfindingAlgorithm> pathfindingAlgorithms)
        {
            var pathfindingAlgorithmResults = new List<PathfindingAlgorithmResult>();

            foreach (PathfindingAlgorithm pathfindingAlgorithm in pathfindingAlgorithms)
            {
                pathfindingAlgorithmResults.Add(new PathfindingAlgorithmResult
                {
                    PathfindingAlgorithm = pathfindingAlgorithm,
                    ComplexityResults = new List<ComplexityResult>(),
                });
            }

            return pathfindingAlgorithmResults;
        }
    }
}
