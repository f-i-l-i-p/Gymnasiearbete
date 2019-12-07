using Priority_Queue;

namespace Gymnasiearbete.Pathfinding.QueueNodes
{
    class FastQueueNode : FastPriorityQueueNode
    {
        public int Value { get; set; }
        public FastQueueNode(int value)
        {
            Value = value;
        }
    }
}
