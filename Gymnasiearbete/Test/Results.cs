using System.Collections.Generic;
using static Gymnasiearbete.Pathfinding.GraphPruning;
using static Gymnasiearbete.Pathfinding.Pathfinder;

namespace Gymnasiearbete.Test
{
    public class TestResult
    {   
        public List<GraphPruningResult> GraphPruningResults { get; set; }
    }

    public class GraphPruningResult
    {
        public PruningAlgorithm PruningAlgorithm { get; set; }

        public List<PathfindingAlgorithmResult> PathfindingAlgorithmResults { get; set; }
    }

    public class PathfindingAlgorithmResult
    {
        public PathfindingAlgorithm PathfindingAlgorithm { get; set; }
        public List<ComplexityResult> ComplexityResults { get; set; }
    }

    public class ComplexityResult
    {
        public double Complexity { get; set; }
        public List<SizeResult> SizeResults { get; set; }
    }

    public class SizeResult
    {
        public int GraphSize { get; set; }
        public AverageSearchResult AverageSearchResult { get; set; }
        public AverageGraphPruningTime AverageGraphPruningTime { get; set; }
        public List<SizeRepeatResult> SizeRepeatResults { get; set;}
    }

    public class SizeRepeatResult
    {
        public int GraphSizeRepet { get; set; }
        public List<SearchResult> SearchResults { get; set; }
        public double GraphPruningTime { get; set; }
    }

    public class AverageSearchResult
    {
        public SearchResult MeanSearchResult { get; set; }
        public SearchResult MedianSearchResult { get; set; }
    }

    public class SearchResult
    {
        public double SearchTime { get; set; }
        public int ExplordedNodes { get; set; }
        public double ExploredRatio { get; set; }
    }

    public class AverageGraphPruningTime
    {
        public double MeanPruningTime { get; set; }
        public double MedianPruningTime { get; set; }
    }
}
