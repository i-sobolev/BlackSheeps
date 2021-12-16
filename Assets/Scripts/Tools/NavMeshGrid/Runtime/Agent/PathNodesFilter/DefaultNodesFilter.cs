namespace NavMeshGrid
{
    public class DefaultNodesFilter : IPathNodesFilter
    {
        public bool NodeMathes(NavMeshGridNode node) => true;
    }
}