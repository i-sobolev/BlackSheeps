using NavMeshGrid;
using UnityEngine;

namespace BlackSheeps
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private NavMeshGrid.NavMeshGrid _navMeshGrid;
        [SerializeField] private Unit _testUnit;

        private NavMeshGridNode _closestNode = null;

        private void OnDrawGizmos()
        {
            if (_closestNode)
                Gizmos.DrawCube(_closestNode.Position, Vector2.one * 1f);
        }

        private void OnEnable()
        {
            _closestNode = FindClosestNode();
        }

        public void Update()
        {
            var horizontalInput = Input.GetAxisRaw("Horizontal");
            var verticalInput = Input.GetAxisRaw("Vertical");

            if (horizontalInput != 0 || verticalInput != 0)
            {
                var positionDelta = new Vector2(horizontalInput, verticalInput).normalized * 0.08f;
                transform.position += (Vector3)positionDelta;

                var lastNodeIndex = _closestNode.Index;
                _closestNode = FindClosestNode();

                if (lastNodeIndex != _closestNode.Index)
                    _testUnit.MoveTo(_closestNode);
            }
        }

        private NavMeshGridNode FindClosestNode()
        {
            float minDistance = Mathf.Infinity;
            NavMeshGridNode closestNode = null;

            foreach (var node in _navMeshGrid.Nodes)
            {
                var currentDistance = Vector2.Distance(transform.position, node.Position);

                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    closestNode = node;
                }
            }

            return closestNode;
        }
    }
}