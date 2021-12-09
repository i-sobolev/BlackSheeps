using DG.Tweening;
using NavMeshGrid;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlackSheeps
{
    public class Unit : NavMeshGridAgent
    {
        private List<NavMeshGridNode> _pathNodesQueue = new List<NavMeshGridNode>();

        private NavMeshGridNode _targetNode = null;

        private Tween _currentMovingAnimation = null;

        private bool PathNodesEnded => _pathNodesQueue.Count <= 0;

        private void Start()
        {
            _currentMovingAnimation = DOTween.Sequence();
        }

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
                BuildMoveSequence();
        }

        public void BuildMoveSequence()
        {
            _pathNodesQueue = new List<NavMeshGridNode>(_currentPath.ResultPathNodes);
            StartMoving();
        }

        public void StartMoving()
        {
            transform.DOKill();

            transform.DOPath(
                path: _currentPath.ResultPathNodes.Select(x => (Vector3)x.Position).ToArray(),
                1f * _currentPath.ResultPathNodes.Count,
                pathMode: PathMode.TopDown2D,
                pathType: PathType.CatmullRom)
                
                .onWaypointChange += (wayPointIndex) =>
                {
                    _currentNode = _pathNodesQueue[wayPointIndex];
                };
        }

        public override void LinkToGridNode(NavMeshGridNode node)
        {
            _currentNode = node;
        }
    }
}