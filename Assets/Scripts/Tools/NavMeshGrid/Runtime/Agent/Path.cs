#pragma warning disable
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NavMeshGrid
{
    public class Path
    {
        public List<NavMeshGridNode> ResultPathNodes { get; private set; } = new List<NavMeshGridNode>();

        public bool IsFound => ResultPathNodes != null && ResultPathNodes.Count > 0;

        public void Find(NavMeshGridNode from, NavMeshGridNode to)
        {
            var startNode = from;
            var targetNode = to;

            var totalCost = 0;

            var reachable = new List<PathNode>() { new PathNode(from, totalCost) };
            var explored = new List<NavMeshGridNode>();

            while (reachable.Count > 0)
            {
                var currentNode = ChooseNode(reachable, targetNode);

                if (currentNode.Node == targetNode)
                    ResultPathNodes = BuildPath(new PathNode(targetNode, 0) { PreviousNode = currentNode });

                reachable.Remove(currentNode);
                explored.Add(currentNode.Node);

                ++totalCost;

                var newReachable = currentNode.Node.AllNeighboringNodes.Select(node => new PathNode(node, totalCost));

                foreach (var newReachableNode in newReachable)
                {
                    if (newReachableNode.Node && !explored.Contains(newReachableNode.Node) && !reachable.Contains(reachable.Find((x) => x.Node == newReachableNode.Node)))
                    {
                        //if (newReachableNode.Node.Data.IsEmpty() == false)
                        //    continue;

                        newReachableNode.PreviousNode = currentNode;
                        reachable.Add(newReachableNode);
                    }
                }
            }
        }

        private PathNode ChooseNode(IEnumerable<PathNode> pathNodes, NavMeshGridNode endNode)
        {
            var minCost = Mathf.Infinity;
            PathNode bestNode = null;

            foreach (var node in pathNodes)
            {
                var nodeCost = node.Cost;

                var costToEndNode = Vector2Int.Distance(
                    a: new Vector2Int(node.Node.Index.Row, node.Node.Index.Column),
                    b: new Vector2Int(endNode.Index.Row, endNode.Index.Column));

                var totalCost = nodeCost + costToEndNode;

                if (totalCost < minCost)
                {
                    minCost = totalCost;
                    bestNode = node;
                }
            }

            return bestNode;
        }

        private List<NavMeshGridNode> BuildPath(PathNode endNode)
        {
            var path = new List<NavMeshGridNode>();
            var currentNode = endNode;

            while (currentNode.PreviousNode != null)
            {
                path.Add(currentNode.Node);
                currentNode = currentNode.PreviousNode;
            }

            path.Reverse();

            return path;
        }
    }
}