using System.Collections.Generic;
using static Gymnasiearbete.Pathfinding.Pathfinder;

namespace Gymnasiearbete.Test
{
    class TestResult
    {
        public int GraphSize { get; set; }
        public List<GraphResult> GraphResults { get; set; }
    }

    class GraphResult
    {
        public string GraphName { get; set; }
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
