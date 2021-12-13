using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavMeshGrid
{
    public class NavMeshGridNode : ScriptableObject
    {
        private NavMeshGridNodeData _data;

        [SerializeField] private Index _index;

        [SerializeField] private NavMeshGridNode[] _neighboringNodes;
        [SerializeField] private Vector2 _position;
        [SerializeField] private Vector2 _customOffset = Vector2.zero;

        public NavMeshGridNodeData Data => _data;
        public Index Index => new Index(_index);

        public Vector2 Position => _position + _customOffset;
        public Vector2 PositionWithoutOffset => _position;
        public Vector2 CustomOffset => _customOffset;

        public IEnumerable<NavMeshGridNode> AllNeighboringNodes
        {
            get
            {
                var result = new List<NavMeshGridNode>();

                foreach (var node in _neighboringNodes)
                    if (node != null)
                        result.Add(node);

                return result;
            }
        }
        public IEnumerable<Side> AllEmptyNeighboringNodesSides
        {
            get
            {
                var result = new List<Side>();

                foreach (var side in Enum.GetValues(typeof(Side)).Cast<Side>())
                    if (!TryGetNeighboringNodeBySide(side, out var _))
                        result.Add(side);

                return result;
            }
        }

        public NavMeshGridNode Initialize(Index index)
        {
            _neighboringNodes ??= new NavMeshGridNode[8];
            _index = index;

            return this;
        }

        public void RemoveNeighboringNode(NavMeshGridNode nodeToRemove)
        {
            for (int i = 0; i < _neighboringNodes.Length; i++)
            {
                if (_neighboringNodes[i] == nodeToRemove)
                {
                    _neighboringNodes[i] = null;
                    break;
                }
            }
        }

        public NavMeshGridNode AddNeighboringNode(Side neighboringNodeSide, NavMeshGridNode newNode)
        {
            if (_neighboringNodes[(int)neighboringNodeSide] != null)
                return null;

            _neighboringNodes[(int)neighboringNodeSide] = newNode;

            newNode.AddNeighboringNode(neighboringNodeSide.OppositeSide(), this);

            return newNode;
        }

        public bool TryGetNeighboringNodeBySide(Side side, out NavMeshGridNode resultNode)
        {
            resultNode = _neighboringNodes[(int)side];
            return resultNode != null;
        }

        public void SetPosition(Vector2 position) => _position = position;

        public void SetCustomOffset(Vector2 offset, float maxLenght = 0)
        {
            _customOffset = maxLenght != 0 ? Vector2.ClampMagnitude(offset, maxLenght) : offset;
        }
    }
}