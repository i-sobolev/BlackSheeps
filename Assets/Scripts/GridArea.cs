using NavMeshGrid;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlackSheeps
{
    public class GridArea : NavMeshGridAgent
    {
        public event Action<NavMeshGridAgent> AgentEnteredArea; 
        public event Action<NavMeshGridAgent> AgentExitArea; 

        [SerializeField] private NavMeshGrid.NavMeshGrid _navMeshGrid;

        private List<NavMeshGridNode> _nodesInArea;
        private List<NavMeshGridAgent> _agentsInArea = new List<NavMeshGridAgent>();
        
        [SerializeField] private int _radius = 2;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var gizmosColor = Color.red;
            gizmosColor.a = 0.7f;

            Gizmos.color = gizmosColor;

            _nodesInArea.ForEach(node => Gizmos.DrawCube(node.Position, Vector2.one * 0.6f));
        }
#endif

        public override void LinkToGridNode(NavMeshGridNode node, bool snap = false)
        {
            _currentNode = node;

            RefreshNodesInArea();
            CheckObjectsInArea();

            if (snap)
                transform.position = node.Position;
        }

        public void RefreshNodesInArea()
        {
            _currentNode = FindClosestNode();

            foreach (var oldNode in _nodesInArea)
            {
                oldNode.AgentLinked -= OnNodeDataChanged;
                oldNode.AgentRemoved -= OnNodeDataChanged;
            }

            _nodesInArea = new List<NavMeshGridNode>() { _currentNode };
            var nodesToAdd = new List<NavMeshGridNode>();

            for (int i = _radius; i > 0; i--)
            {
                foreach (var node in _nodesInArea)
                {
                    foreach (var neighboringNode in node.AllNeighboringNodes)
                    {
                        if (!_nodesInArea.Contains(neighboringNode) && !nodesToAdd.Contains(neighboringNode) && neighboringNode != _currentNode)
                        {
                            nodesToAdd.Add(neighboringNode);
                        }
                    }
                }

                _nodesInArea.AddRange(nodesToAdd);
                nodesToAdd.Clear();
            }

            foreach (var node in _nodesInArea)
            {
                node.AgentLinked += OnNodeDataChanged;
                node.AgentRemoved += OnNodeDataChanged;
            }
        }

        public void CheckObjectsInArea()
        {
            foreach (var node in _nodesInArea)
            {
                var agentOnNode = node.AgentOnNode;

                if (agentOnNode != null)
                {
                    if (!_agentsInArea.Contains(agentOnNode))
                    {
                        _agentsInArea.Add(agentOnNode);
                        AgentEnteredArea?.Invoke(agentOnNode);

                        Debug.Log("Agent entered area!");
                    }
                }
            }

            var agentsToRemove = new List<NavMeshGridAgent>();

            foreach (var agent in _agentsInArea)
            {
                var nodeWithAgentExist = _nodesInArea.Exists(node => node.AgentOnNode == agent);

                if (!nodeWithAgentExist)
                {
                    agentsToRemove.Add(agent);
                    AgentExitArea?.Invoke(agent);
                    
                    Debug.Log("Agent exit area!");
                }
            }

            foreach (var agent in agentsToRemove)
                _agentsInArea.Remove(agent);
        }

        private void OnNodeDataChanged(NavMeshGridAgent agent)
        {
            CheckObjectsInArea();
        }

        private NavMeshGridNode FindClosestNode()
        {
            float minDistance = Mathf.Infinity;
            NavMeshGridNode closestNode = null;

            foreach (var node in _navMeshGrid.Nodes)
            {
                var currentDistance = Vector2.Distance(transform.position, node.Position);

                if (currentDistance < minDistance)
                {
                    minDistance = currentDistance;
                    closestNode = node;
                }
            }

            return closestNode;
        }
    }
}