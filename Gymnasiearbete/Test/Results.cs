using System.Collections.Generic;
using static Gymnasiearbete.Pathfinding.GraphPruning;
using static Gymnasiearbete.Pathfinding.Pathfinder;

namespace Gymnasiearbete.Test
{
    class TestResult
    {   
        public List<GraphPruningResult> GraphPruningResults { get; set; }
    }

    class GraphPruningResult
    {
        public PruningAlgorithm PruningAlgorithm { get; set; }

        public List<PathfindingAlgorithmResult> PathfindingAlgorithmResults { get; set; }
    }

    class PathfindingAlgorithmResult
    {
        public PathfindingAlgorithm PathfindingAlgorithm { get; set; }
        public List<ComplexityResult> ComplexityResults { get; set; }
    }

    class ComplexityResult
    {
        public double Complexity { get; set; }
        public List<SizeResult> SizeResults { get; set; }
    }

    class SizeResult
    {
        public int GraphSize { get; set; }
        public AverageSearchResult AverageSearchResult { get; set; }
        public AverageGraphPruningTime AverageGraphPruningTime { get; set; }
        public List<SizeRepeatResult> SizeRepeatResults { get; set;}
    }

    class SizeRepeatResult
    {
        public int GraphSizeRepet { get; set; }
        public List<SearchResult> SearchResults { get; set; }
        public double GraphPruningTime { get; set; }
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

    class AverageGraphPruningTime
    {
        public double MeanPruningTime { get; set; }
        public double MedianPruningTime { get; set; }
    }
}
