using System.Collections.Generic;
using static Gymnasiearbete.Pathfinding.GraphOptimization;
using static Gymnasiearbete.Pathfinding.Pathfinder;

namespace Gymnasiearbete.Test
{
    class TestResult
    {   
        public List<GraphOptimizationResult> GraphOptimizationResults { get; set; }
    }

    class GraphOptimizationResult
    {
        public OptimizationType OptimizationType { get; set; }

        public List<SearchTypeResult> SearchTypeResults { get; set; }
    }

    class SearchTypeResult
    {
        public SearchType SearchType { get; set; }
        public List<OpennessResult> OpennessResults { get; set; }
    }

    class OpennessResult
    {
        public double Openness { get; set; }
        public List<SizeResult> SizeResults { get; set; }
    }

    class SizeResult
    {
        public int GraphSize { get; set; }
        public AverageSearchResult AverageSearchResult { get; set; }
        public List<SizeRepeatResult> SizeRepeatResults { get; set;}
    }

    class SizeRepeatResult
    {
        public int GraphSizeRepet { get; set; }
        public List<SearchResult> SearchResults { get; set; }
    }

    class AverageSearchResult
    {
        public SearchResult MeanSearchResult { get; set; }
        public SearchResult MedianSearchResult { get; set; }
    }

    class SearchResult
    {
        public double SearchTime { get; set; }
        public int ExplordedNodes { get; set; }
        public double ExploredRatio { get; set; }
    }
}
