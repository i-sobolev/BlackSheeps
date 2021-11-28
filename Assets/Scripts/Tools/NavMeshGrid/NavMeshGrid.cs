using System.Collections.Generic;
using UnityEngine;

public class NavMeshGrid : MonoBehaviour
{
    private List<NavMeshGridNode> _nodes;
    public IEnumerable<NavMeshGridNode> Nodes => _nodes;

    private void OnValidate()
    {
        _nodes = new List<NavMeshGridNode>();

        var node11 = new NavMeshGridNode();
        var node12 = new NavMeshGridNode();
        var node21 = new NavMeshGridNode();
        var node22 = new NavMeshGridNode();

        node11.AddNeighboringNode(NeighboringNodeSide.Right, node12);
        node11.AddNeighboringNode(NeighboringNodeSide.Lower, node21);
        node11.AddNeighboringNode(NeighboringNodeSide.LowerRight, node22);

        node12.AddNeighboringNode(NeighboringNodeSide.LowerLeft, node21);

        node22.AddNeighboringNode(NeighboringNodeSide.Upper, node12);
        node22.AddNeighboringNode(NeighboringNodeSide.Left, node21);

        _nodes = new List<NavMeshGridNode>()
        {
            node11,
            node12,
            node21,
            node22
        };
    }
}
