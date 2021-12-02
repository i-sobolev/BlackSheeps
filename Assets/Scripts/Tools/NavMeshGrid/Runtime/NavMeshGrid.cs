using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavMeshGrid : MonoBehaviour
{
    [HideInInspector][SerializeField] private List<NavMeshGridNode> _nodes = new List<NavMeshGridNode>();
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
}