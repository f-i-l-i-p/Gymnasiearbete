using System.Collections.Generic;
using static Gymnasiearbete.Pathfinding.Pathfinder;

namespace Gymnasiearbete.Test
{
    class TestResults
    {   
        public List<GraphOptimizationResults> GraphOptimizationResults { get; set; }
    }

    public enum OptimizationType { None, Shrinked }
    class GraphOptimizationResults
    {
        public OptimizationType OptimizationType { get; set; }

        public List<SearchTypeResults> SearchTypeResults { get; set; }
    }

    /// <summary>
    /// Contains all the results for a SearchType on a set of graphs
    /// </summary>
    class SearchTypeResults
    {
        public SearchType SearchType { get; set; }
        public List<OpennessResults> OpennessResults { get; set; }
    }

    class OpennessResults
    {
        public double Openness { get; set; }
        public List<SizeResults> SizeResults { get; set; }
    }

    class SizeResults
    {
        public int GraphSize { get; set; }
        public AverageSearchResult AverageSearchResult { get; set; }
        public List<SizeRepeatResults> SizeRepeatResults { get; set;}
    }

    class SizeRepeatResults
    {
        public int GraphSizeRepet { get; set; }
        public List<SearchTypeResults> SearchTypeResults { get; set; }
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
