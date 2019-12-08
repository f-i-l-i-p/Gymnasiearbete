using System.Collections.Generic;
using static Gymnasiearbete.Pathfinding.Pathfinder;

namespace Gymnasiearbete.Test
{
    class TestResults
    {
        // public Date // TODO: add test date
        public List<OpennessResults> OpennesResults { get; set; }
    }

    class OpennessResults
    {
        public double Openness { get; set; }
        public List<SizeResults> SizeResults { get; set; }
    }

    class SizeResults
    {
        public int GraphSize { get; set; }
        public List<SizeRepeatResults> SizeRepeatResults { get; set;}
        public List<AverageSearchTypeResult> AverageSearchTypeResults { get; set; }
    }

    class SizeRepeatResults
    {
        public int GraphSizeRepet { get; set; }
        public List<SearchTypeResults> SearchTypeResults { get; set; }
    }

    class SearchTypeResults
    {
        public SearchType SearchType { get; set; }
        public List<SearchResult> SearchResults { get; set; }
    }

    class SearchResult
    {
        public double SearchTime { get; set; }
        public int ExplordedNodes { get; set; }
        public double ExploredRatio { get; set; }
    }

    class AverageSearchTypeResult
    {
        public SearchType SearchType { get; set; }
        public SearchResult MeanSearchResult { get; set; }
        public SearchResult MedianSearchResult { get; set; }
    }
}
