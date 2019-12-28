using System.Collections.Generic;

namespace Gymnasiearbete.Graphs
{
    class Graph
    {
        /// <summary>
        /// All nodes in the graph.
        /// </summary>
        public List<Node> Nodes { get; set; }

        public Graph()
        {
            Nodes = new List<Node>();
        }

        /// <summary>
        /// Adds an edge between two nodes.
        /// </summary>
        /// <param name="weight">Weight to travel between nodes.</param>
        public void AddEdge(Node node1, Node node2, float weight = 1)
        {
            Nodes[node1.Id].Adjacents.Add(new Adjacent { Id = node2.Id, Weight = weight });
            Nodes[node2.Id].Adjacents.Add(new Adjacent { Id = node1.Id, Weight = weight });
        }

        /// <summary>
        /// Removes edges two nodes.
        /// </summary>
        public void RemoveEdge(Node node1, Node node2)
        {
            Nodes[node1.Id].Adjacents.RemoveAll(x => x.Id == node2.Id);
            Nodes[node2.Id].Adjacents.RemoveAll(x => x.Id == node1.Id);
        }

        /// <summary>
        /// Adds a new node and returns its id.
        /// </summary>
        /// <param name="position">The node's real position.</param>
        /// <returns>The new node's id.</returns>
        public int AddNode(Position position)
        {
            var index = Nodes.FindIndex(e => e == null);

            // If no empty list position exists
            if (index == -1)
            {
                Nodes.Add(new Node
                {
                    Id = Nodes.Count,
                    Position = position,
                    Adjacents = new List<Adjacent>()
                });
                return Nodes.Count - 1;
            }
            else
            {
                Nodes[index] = new Node
                {
                    Id = index,
                    Position = position,
                    Adjacents = new List<Adjacent>()
                };
                return index;
            }
        }

        /// <summary>
        /// Removes a node and all edges to it.
        /// </summary>
        /// <param name="node">Node to remove.</param>
        public void RemoveNode(Node node)
        {
            // remove the node as adjacent to other nodes
            Nodes.ForEach(n => n.Adjacents.RemoveAll(x => x.Id == node.Id));

            // remove the node
            Nodes[node.Id] = null;
        }
    }
}
