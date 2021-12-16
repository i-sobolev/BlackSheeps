namespace NavMeshGrid
{
    public interface IPathNodesFilter
    {
        public bool NodeMathes(NavMeshGridNode node);
    }
}