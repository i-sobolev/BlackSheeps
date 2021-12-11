using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavMeshGrid
{
    public class NavMeshGrid : MonoBehaviour
    {
        [SerializeField] private List<NavMeshGridNode> _nodes;
        [SerializeField] private Vector2 _nodesHorizontalOffset;
        [SerializeField] private Vector2 _nodesVerticalOffset;

        public Vector2 NodesVerticalOffset => _nodesVerticalOffset;
        public Vector2 NodesHorizontalOffset => _nodesHorizontalOffset;
        public IEnumerable<NavMeshGridNode> Nodes => _nodes;
        public NavMeshGridNode RootNode => _nodes[0];

        private void Reset()
        {
            _nodes.Clear();
            _nodes.Add(ScriptableObject.CreateInstance<NavMeshGridNode>().Initialize(new Index(0, 0)));
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

            foreach (var side in System.Enum.GetValues(typeof(Side)).Cast<Side>())
                TryAddNeghboringNode(side);

            return newNode;
        }

        public void RemoveNode(NavMeshGridNode selectedNode)
        {
            foreach (var node in selectedNode.AllNeighboringNodes)
                node.RemoveNeighboringNode(selectedNode);

            _nodes.Remove(selectedNode);
        }

        public void SetOffsets(Vector2 horizontal, Vector2 vertical)
        {
            _nodesHorizontalOffset = horizontal;
            _nodesVerticalOffset = vertical;
        }

        public void RefreshNodesPositions()
        {
            void TryGetNodeAndSetPositionBySide(NavMeshGridNode node, Side neededNodeSide)
            {
                if (node.TryGetNeighboringNodeBySide(neededNodeSide, out var neededNode))
                    neededNode.SetPosition(node.Position + GetOffsetBySide(neededNodeSide));
            }

            foreach (var node in _nodes)
            {
                TryGetNodeAndSetPositionBySide(node, Side.Left);
                TryGetNodeAndSetPositionBySide(node, Side.Right);
                TryGetNodeAndSetPositionBySide(node, Side.Upper);
                TryGetNodeAndSetPositionBySide(node, Side.Lower);
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
                _ => Vector2.zero,
            };
        }

    }
}