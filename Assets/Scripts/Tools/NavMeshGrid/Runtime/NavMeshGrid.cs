using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavMeshGrid : MonoBehaviour
{
    [SerializeField] private List<NavMeshGridNode> _nodes;
    public IEnumerable<NavMeshGridNode> Nodes => _nodes;

    [SerializeField] private Vector2 _nodesHorizontalOffset;
    public Vector2 NodesHorizontalOffset => _nodesHorizontalOffset;

    [SerializeField] private Vector2 _nodesVerticalOffset;
    public Vector2 NodesVerticalOffset => _nodesVerticalOffset;

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