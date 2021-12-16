#pragma warning disable
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace NavMeshGrid
{
    public class Path
    {
        public List<NavMeshGridNode> ResultPathNodes { get; private set; } = new List<NavMeshGridNode>();

        public bool IsFound => ResultPathNodes != null && ResultPathNodes.Count > 0;

        public void Find(NavMeshGridNode from, NavMeshGridNode to, IPathNodesFilter filter = null)
        {
            Profiler.BeginSample("PathFinding");
            filter ??= new DefaultNodesFilter();

            var startNode = from;
            var targetNode = to;

            var totalCost = 0;

            var reachable = new List<PathNode>() { new PathNode(from, totalCost) };
            var explored = new List<Index>();

            while (reachable.Count > 0)
            {
                Profiler.BeginSample("ChoosingNode");
                var currentNode = ChooseNode(reachable, targetNode);
                Profiler.EndSample();

                if (currentNode.Node == targetNode)
                {
                    ResultPathNodes = BuildPath(new PathNode(targetNode, 0) { PreviousNode = currentNode });
                    break;
                }

                reachable.Remove(currentNode);
                explored.Add(currentNode.Node.Index);

                ++totalCost;

                Profiler.BeginSample("NewReachebleSelecting");
                var newReachable = currentNode.Node.AllNeighboringNodes
                    .Where(node => !explored.Contains(node.Index))
                    .Where(node => !reachable.Exists(x => x.Node == node))
                    .Select(node => new PathNode(node, totalCost));
                Profiler.EndSample();

                Profiler.BeginSample("FindingPath");
                foreach (var newReachableNode in newReachable)
                {
                    Profiler.BeginSample("Filter");
                    var nodeMatches = filter.NodeMathes(newReachableNode.Node);
                    Profiler.EndSample();

                    if (nodeMatches)
                    {
                        newReachableNode.PreviousNode = currentNode;
                        reachable.Add(newReachableNode);
                    }
                }
                Profiler.EndSample();
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