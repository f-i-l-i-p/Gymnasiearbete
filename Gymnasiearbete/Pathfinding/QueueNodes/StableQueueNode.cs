using Priority_Queue;

namespace Gymnasiearbete.Pathfinding.QueueNodes
{
    class StableQueueNode : StablePriorityQueueNode
    {
        public int Value { get; set; }
        public StableQueueNode(int value)
        {
            Value = value;
        }
    }
}
