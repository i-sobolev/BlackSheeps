using DG.Tweening;
using NavMeshGrid;
using System.Linq;
using UnityEngine;

namespace BlackSheeps
{
    public class Unit : NavMeshGridAgent
    {
        private const float MovingSpeed = 1f;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_currentPath != null && _currentPath.ResultPathNodes.Count > 0)
            {
                Gizmos.color = Color.blue;

                for (int i = 0; i < _currentPath.ResultPathNodes.Count - 1; i++)
                    Gizmos.DrawLine(_currentPath.ResultPathNodes[i].Position, _currentPath.ResultPathNodes[i + 1].Position);
            }
        }
#endif
        public void MoveTo(NavMeshGridNode node)
        {
            BuildPath(node);

            if (_currentPath.IsFound && _currentPath.ResultPathNodes.Count > 0)
                StartMoving();
        }

        public void StartMoving()
        {
            transform.DOKill();
            transform.DOPath(
                path: _currentPath.ResultPathNodes.Select(node => (Vector3)node.Position).ToArray(),
                duration: MovingSpeed * _currentPath.ResultPathNodes.Count,
                pathMode: PathMode.TopDown2D,
                pathType: PathType.CatmullRom)
                .OnWaypointChange(OnWayPointChanged)
                .SetId(transform);
        }

        private void OnWayPointChanged(int nodeIndex)
        {
            LinkToGridNode(_currentPath.ResultPathNodes[nodeIndex]);
        }
    }
}