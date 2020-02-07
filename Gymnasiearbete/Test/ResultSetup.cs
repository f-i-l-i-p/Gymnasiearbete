using System.Collections.Generic;
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
}
