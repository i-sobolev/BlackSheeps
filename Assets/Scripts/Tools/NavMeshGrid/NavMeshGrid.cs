using System.Collections.Generic;
using UnityEngine;

public class NavMeshGrid : MonoBehaviour
{
    private List<NavMeshGridNode> _nodes = new List<NavMeshGridNode>();
    public IEnumerable<NavMeshGridNode> Nodes => _nodes;

    public NavMeshGridNode RootNode => _nodes[0];

    private void Reset()
    {
        _nodes.Clear();
        _nodes.Add(new NavMeshGridNode(new Index(0, 0)));
    }

    [ExecuteAlways]
    private void UpdatePosition() => RootNode.SetPosition(transform.position);

    public bool GetNodeByIndex(Index index, out NavMeshGridNode node)
    {
        node = _nodes.Find(node => node.Index == index);
        return node != null;
    }

    public void AddNewNode(Index index)
    {
        var newNode = new NavMeshGridNode(index);
        
        _nodes.Add(newNode);

        var newNodeIndex = newNode.Index;
     
        if (GetNodeByIndex(newNodeIndex.Left(), out var leftNode))
            newNode.AddNeighboringNode(NeighboringNodeSide.Left, leftNode);

        if (GetNodeByIndex(newNodeIndex.Right(), out var rightNode))
            newNode.AddNeighboringNode(NeighboringNodeSide.Right, rightNode);

        if (GetNodeByIndex(newNodeIndex.Upper(), out var upperNode))
            newNode.AddNeighboringNode(NeighboringNodeSide.Upper, upperNode);

        if (GetNodeByIndex(newNodeIndex.Lower(), out var lowerNode))
            newNode.AddNeighboringNode(NeighboringNodeSide.Lower, lowerNode);

        if (GetNodeByIndex(newNodeIndex.UpperLeft(), out var upperLeftNode))
            newNode.AddNeighboringNode(NeighboringNodeSide.UpperLeft, upperLeftNode);

        if (GetNodeByIndex(newNodeIndex.UpperRight(), out var upperRightNode))
            newNode.AddNeighboringNode(NeighboringNodeSide.UpperRight, upperRightNode);

        if (GetNodeByIndex(newNodeIndex.LowerLeft(), out var lowerLeftNode))
            newNode.AddNeighboringNode(NeighboringNodeSide.LowerLeft, lowerLeftNode);

        if (GetNodeByIndex(newNodeIndex.LowerRight(), out var lowerRightNode))
            newNode.AddNeighboringNode(NeighboringNodeSide.LowerRight, lowerRightNode);
    }
}
