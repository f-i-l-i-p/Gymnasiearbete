using System.Collections.Generic;

namespace Gymnasiearbete.Graphs
{
    class Node
    {
        /// <summary>
        /// This node's id.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// This node's real position.
        /// </summary>
        public Position Position { get; set; }
        /// <summary>
        /// List of all adjacent nodes to this node.
        /// </summary>
        public List<Adjacent> Adjacents { get; set; }
    }
}
