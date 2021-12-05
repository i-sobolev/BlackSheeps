using DG.Tweening;
using NavMeshGrid;
using UnityEngine;

namespace BlackSheeps
{
    public class Unit : NavMeshGridAgent
    {
        private Tween _currentAnimation;

        private void OnDrawGizmos()
        {
            if (_currentPath != null && _currentPath.PathNodes.Count > 0)
            {
                Gizmos.color = Color.white;

                for (int i = 0; i < _currentPath.PathNodes.Count - 1; i++)
                    Gizmos.DrawLine(_currentPath.PathNodes[i].Position, _currentPath.PathNodes[i + 1].Position);
            }
        }

        public void MoveTo(NavMeshGridNode node)
        {
            BuildPath(node);
            BuildMoveSequence(_currentPath);
        }

        public void BuildMoveSequence(Path path)
        {
            _currentAnimation?.Kill();

            var movingSequence = DOTween.Sequence();
                
            foreach (var node in path.PathNodes)
                movingSequence.Append(MoveToNode(node));

            movingSequence.SetEase(Ease.Linear);

            _currentAnimation = movingSequence;
        }

        public Tween MoveToNode(NavMeshGridNode node)
        {
            var tween = transform.DOMove(node.Position, 0.7f).SetEase(Ease.Linear);
            tween.onComplete += () => LinkToGridNode(node);
            return tween;
        }

        public override void LinkToGridNode(NavMeshGridNode node)
        {
            _currentNode = node;
        }
    }
}