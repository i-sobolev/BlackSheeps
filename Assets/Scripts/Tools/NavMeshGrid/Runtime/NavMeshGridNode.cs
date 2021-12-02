using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavMeshGridNode : ScriptableObject
{
    [HideInInspector] [SerializeField] private Index _index;

    [HideInInspector] [SerializeField] private NavMeshGridNode[] _neighboringNodes;
    [HideInInspector] [SerializeField] private Vector2 _position;

    public NavMeshGridNode Initialize(Index index)
    {
        _neighboringNodes = new NavMeshGridNode[8];
        _index = index;

        return this;
    }

    public Index Index => new Index(_index);

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

    public Vector2 Position => _position;

    public NavMeshGridNode AddNeighboringNode(Side neighboringNodeSide, NavMeshGridNode newNode)
    {
        if (_neighboringNodes[(int)neighboringNodeSide] != null)
            return null;

        _neighboringNodes[(int)neighboringNodeSide] = newNode;

        newNode.AddNeighboringNode(GetOppositeNodeSide(neighboringNodeSide), this);
        
        return newNode;
    }

    public bool TryGetNeighboringNodeBySide(Side side, out NavMeshGridNode resultNode)
    {
        resultNode = _neighboringNodes[(int)side];
        return resultNode != null;
    }

    public void SetPosition(Vector2 position) => _position = position;

    private Side GetOppositeNodeSide(Side currentSide)
    {
        var currentSideId = (int)currentSide;
        return (Side)(currentSideId % 2 == 0 ? currentSideId + 1 : currentSideId - 1);
    }
}