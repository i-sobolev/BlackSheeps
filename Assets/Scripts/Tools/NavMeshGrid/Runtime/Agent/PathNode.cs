namespace NavMeshGrid
{
    public class PathNode
    {
        public PathNode PreviousNode { get; set; } = null;
        public NavMeshGridNode Node { get; set; }
        public int Cost { get; set; }

        public PathNode(NavMeshGridNode node, int cost)
        {
            Node = node;
            Cost = cost;
        }
    }
}