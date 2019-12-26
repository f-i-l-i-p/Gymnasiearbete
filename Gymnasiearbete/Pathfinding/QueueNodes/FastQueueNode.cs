using Gymnasiearbete.Graphs;
using Priority_Queue;

namespace Gymnasiearbete.Pathfinding.QueueNodes
{
    class FastQueueNode : FastPriorityQueueNode
    {
        public Node Value { get; set; }
        public FastQueueNode(Node value)
        {
            Value = value;
        }
    }
}
