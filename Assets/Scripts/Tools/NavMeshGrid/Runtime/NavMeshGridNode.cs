using System;
using System.Collections.Generic;
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

    public IEnumerable<NeighboringNodeSide> AllEmptyNeighboringNodesSides
    {
        get
        {
            var result = new List<NeighboringNodeSide>();

            foreach (var side in Enum.GetValues(typeof(NeighboringNodeSide)))
                if (!TryGetNeighboringNodeBySide((NeighboringNodeSide)side, out var _))
                    result.Add((NeighboringNodeSide)side);

            return result;
        }
    }

    public Vector2 Position => _position;

    public NavMeshGridNode AddNeighboringNode(NeighboringNodeSide neighboringNodeSide, NavMeshGridNode newNode)
    {
        if (_neighboringNodes[(int)neighboringNodeSide] != null)
            return null;

        _neighboringNodes[(int)neighboringNodeSide] = newNode;

        newNode.AddNeighboringNode(GetOppositeNodeSide(neighboringNodeSide), this);
        
        return newNode;
    }

    public bool TryGetNeighboringNodeBySide(NeighboringNodeSide side, out NavMeshGridNode resultNode)
    {
        resultNode = _neighboringNodes[(int)side];
        return resultNode != null;
    }

    public void SetPosition(Vector2 position) => _position = position;

    private NeighboringNodeSide GetOppositeNodeSide(NeighboringNodeSide currentSide)
    {
        var currentSideId = (int)currentSide;
        return (NeighboringNodeSide)(currentSideId % 2 == 0 ? currentSideId + 1 : currentSideId - 1);
    }
}