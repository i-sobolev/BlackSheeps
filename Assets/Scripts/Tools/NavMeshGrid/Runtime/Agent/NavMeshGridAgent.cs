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

        public void LinkToGridNode(NavMeshGridNode node, bool snap = false)
        {
            _currentNode = node;

            if (snap)
                transform.position = node.Position;
        }
    }
}