using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavMeshGrid
{
    [Serializable]
    public class NavMeshGrid : MonoBehaviour
    {
        [SerializeField] private List<NavMeshGridNode> _nodes;
        [SerializeField] private Vector2 _nodesHorizontalOffset;
        [SerializeField] private Vector2 _nodesVerticalOffset;

        public Vector2 NodesVerticalOffset => _nodesVerticalOffset;
        public Vector2 NodesHorizontalOffset => _nodesHorizontalOffset;
        public IEnumerable<NavMeshGridNode> Nodes => _nodes;

        public NavMeshGridNode RootNode { get; private set; }
        
        private void Reset()
        {
            _nodes.Clear();

            RootNode = ScriptableObject.CreateInstance<NavMeshGridNode>().Initialize(new Index(0, 0));
            _nodes.Add(RootNode);
        }

        public bool GetNodeByIndex(Index index, out NavMeshGridNode node)
        {
            node = _nodes.Find(node => node.Index == index);
            return node != null;
        }

#if UNITY_EDITOR
        public void SetDataFromModel(NavMeshGridNodeModel[] models)
        {
            Reset();

            foreach (var model in models)
            {
                if (model.Index == RootNode.Index)
                {
                    RootNode.SetPosition(model.Position);
                    transform.position = model.Position;
                    continue;
                }

                var newNode = AddNewNode(model.Index);
                newNode.SetCustomOffset(model.Offset);
                newNode.SetPosition(model.Position);
            }

            SetOffsetsFromNodes();

            void SetOffsetsFromNodes()
            {
                if (RootNode.TryGetNeighboringNodeBySide(Side.Right, out var rightNode))
                    _nodesHorizontalOffset = rightNode.Position - RootNode.Position;
                
                if (RootNode.TryGetNeighboringNodeBySide(Side.Left, out var leftNode))
                    _nodesHorizontalOffset = -(leftNode.Position - RootNode.Position);

                if (RootNode.TryGetNeighboringNodeBySide(Side.Upper, out var upperNode))
                    _nodesVerticalOffset = upperNode.Position - RootNode.Position;

                if (RootNode.TryGetNeighboringNodeBySide(Side.Lower, out var lowerNode))
                    _nodesVerticalOffset = -(lowerNode.Position - RootNode.Position);
            }
        }

        [ContextMenu("Sort nodes list by index")]
        public void SortNodesByIndex()
        {
            _nodes = _nodes.OrderBy(x => x.Index.Row).ThenBy(y => y.Index.Column).ToList();
        }
#endif
        public NavMeshGridNode AddNewNode(Index index)
        {
            var newNode = ScriptableObject.CreateInstance<NavMeshGridNode>().Initialize(index);

            _nodes.Add(newNode);

            void TryAddNeghboringNode(Side side)
            {
                if (GetNodeByIndex(newNode.Index.IndexBySide(side), out var foundedNode))
                    newNode.AddNeighboringNode(side, foundedNode);
            }

            foreach (var side in Enum.GetValues(typeof(Side)).Cast<Side>())
                TryAddNeghboringNode(side);

            return newNode;
        }

        public void RemoveNode(NavMeshGridNode selectedNode)
        {
            foreach (var neighboringNode in selectedNode.AllNeighboringNodes)
                neighboringNode.RemoveNeighboringNode(selectedNode);

            _nodes.Remove(selectedNode);
        }

        public void SetOffsets(Vector2 horizontal, Vector2 vertical)
        {
            _nodesHorizontalOffset = horizontal;
            _nodesVerticalOffset = vertical;
        }


        public void RefreshNodesPositions()
        {
            var reachable = new Queue<NavMeshGridNode>();
            reachable.Enqueue(RootNode);

            var refreshed = new List<NavMeshGridNode>() { RootNode };

            while (reachable.Count > 0)
            {
                var currentNode = reachable.Dequeue();

                foreach (var side in Enum.GetValues(typeof(Side)).Cast<Side>())
                    TryGetNodeAndSetPositionBySide(currentNode, side);
            }

            void TryGetNodeAndSetPositionBySide(NavMeshGridNode node, Side neededNodeSide)
            {
                if (node.TryGetNeighboringNodeBySide(neededNodeSide, out var neededNode))
                {
                    if (refreshed.Contains(neededNode))
                        return;

                    neededNode.SetPosition(node.PositionWithoutOffset + GetOffsetBySide(neededNodeSide));
                    refreshed.Add(neededNode);
                    reachable.Enqueue(neededNode);
                }
            }

        }

        public Vector2 GetOffsetBySide(Side neighboringNodeSide)
        {
            return neighboringNodeSide switch
            {
                Side.Left => -_nodesHorizontalOffset,
                Side.Right => _nodesHorizontalOffset,
                Side.Lower => -_nodesVerticalOffset,
                Side.Upper => _nodesVerticalOffset,
                Side.UpperLeft => _nodesVerticalOffset -_nodesHorizontalOffset,
                Side.UpperRight => _nodesVerticalOffset + _nodesHorizontalOffset,
                Side.LowerLeft => -_nodesVerticalOffset - _nodesHorizontalOffset,
                Side.LowerRight => -_nodesVerticalOffset + _nodesHorizontalOffset,
                _ => Vector2.zero,
            };
        }
    }
}