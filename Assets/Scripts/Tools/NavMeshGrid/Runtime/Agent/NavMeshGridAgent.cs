using UnityEngine;

namespace NavMeshGrid
{
    public class NavMeshGridAgent : MonoBehaviour
    {
        [SerializeField] protected NavMeshGridNode _currentNode;

        protected Path _currentPath = new Path();

        protected void BuildPath(NavMeshGridNode targetNode)
        {
            _currentPath.Find(_currentNode, targetNode);
            
        }

        public virtual void LinkToGridNode(NavMeshGridNode node)
        {
            _currentNode = node;
            transform.position = node.Position;
        }
    }
}