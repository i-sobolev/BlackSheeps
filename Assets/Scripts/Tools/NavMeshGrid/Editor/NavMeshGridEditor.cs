using UnityEditor;
using UnityEngine;

namespace NavMeshGrid
{
    [CustomEditor(typeof(NavMeshGrid))]
    public class NavMeshGridEditor : Editor
    {
        private bool _changingOffsetsAllowed;

        private Vector3 _lastTargetPosition;

        private NavMeshGridNode _selectedNode = null;

        protected NavMeshGrid Grid => target as NavMeshGrid;


        public void OnSceneGUI()
        {
            DrawTogglesLayout();

            MoveRootNodeToTargetTransforPositins();

            DrawNodesConnections();
            DrawOffsetsHandles();
            DrawNodesIndexes();

            DrawNodesSelectButtons();

            DrawAgentsHandles();
        }

        private void DrawNodesSelectButtons()
        {
            foreach (var node in Grid.Nodes)
            {
                var nodeButtonClicked = Handles.Button(node.Position, Quaternion.identity, 0.2f, 0.2f, Handles.RectangleHandleCap);

                if (nodeButtonClicked)
                    _selectedNode = node;
            };


            if (_selectedNode)
            {
                DrawNewNodesButtons(_selectedNode);

                Handles.BeginGUI();

                var buttonSize = new Vector2(60f, 20f);

                var removeNodeButtonClicked = GUI.Button(
                position: new Rect(HandleUtility.WorldToGUIPoint(_selectedNode.Position + Vector2.up * 0.3f) - buttonSize * 0.5f, buttonSize),
                text: $"-remove");

                if (removeNodeButtonClicked)
                {
                    Grid.RemoveNode(_selectedNode);

                    Debug.Log($"Node {_selectedNode.Index.Column} {_selectedNode.Index.Row} removed");

                    Grid.RefreshNodesPositions();

                    _selectedNode = null;
                }

                Handles.EndGUI();

                SceneView.RepaintAll();
            }

            else
            {
                DrawNewNodesButtonsForAll();
            }
        }

        private void DrawTogglesLayout()
        {
            Handles.BeginGUI();
            {
                GUILayout.BeginArea(new Rect(Vector2.one * 10f, new Vector2(200f, 20f)), new GUIStyle("box"));
                {
                    _changingOffsetsAllowed = GUILayout.Toggle(_changingOffsetsAllowed, "Show nodes offsets handles");
                }
                GUILayout.EndArea();
            }
            Handles.EndGUI();
        }

        private void MoveRootNodeToTargetTransforPositins()
        {
            var currentTargetPosition = Grid.transform.position;

            if (_lastTargetPosition != currentTargetPosition)
            {
                _lastTargetPosition = currentTargetPosition;
                Grid.RootNode.SetPosition(_lastTargetPosition);

                Grid.RefreshNodesPositions();
            }
        }

        private void DrawNodesIndexes()
        {
            foreach (var node in Grid.Nodes)
            {
                Handles.BeginGUI();

                GUI.Label(
                    position: new Rect(HandleUtility.WorldToGUIPoint(node.Position), new Vector2(60, 20)),
                    text: $"{node.Index.Row} {node.Index.Column}");

                Handles.EndGUI();
            }
        }

        private readonly Color _nodesConnectionsColor = new Color(1, 1, 1, 0.1f);

        private void DrawNodesConnections()
        {
            Handles.color = _nodesConnectionsColor;

            foreach (var node in Grid.Nodes)
            {
                foreach (var neighboringNode in node.AllNeighboringNodes)
                {
                    Handles.DrawLine(node.Position, neighboringNode.Position, 1);
                }
            }
        }

        #region AgentsGridLinking
        public void DrawAgentsHandles()
        {
            foreach (var agent in FindObjectsOfType<NavMeshGridAgent>())
            {
                EditorGUI.BeginChangeCheck();
                {
                    agent.transform.position = Handles.FreeMoveHandle(
                        position: agent.transform.position,
                        rotation: Quaternion.identity,
                        size: 0.1f,
                        snap: Vector2.zero,
                        capFunction: Handles.DotHandleCap);

                    if (EditorGUI.EndChangeCheck())
                        LinkAgentToClosestNode(agent);
                }                   
            }
        }

        private void LinkAgentToClosestNode(NavMeshGridAgent agent)
        {
            const float distanceToLink = 0.2f;
            
            foreach (var node in Grid.Nodes)
            {
                var currentDistance = Vector2.Distance(node.Position, agent.transform.position);

                if (currentDistance < distanceToLink)
                    agent.LinkToGridNode(node);
            }
        }
        #endregion

        #region GridEdititg
        private void DrawNewNodesButtonsForAll()
        {
            foreach (var node in Grid.Nodes)
            {
                if (DrawNewNodesButtons(node))
                    break;
            }
        }

        private bool DrawNewNodesButtons(NavMeshGridNode node)
        {
            foreach (var side in node.AllEmptyNeighboringNodesSides)
            {
                if (side.IsDiagonal())
                    continue;

                Handles.BeginGUI();

                var buttonSize = new Vector2(50f, 20f);

                var addNewNodeButtonClicked = GUI.Button(
                    position: new Rect(HandleUtility.WorldToGUIPoint(node.Position + Grid.GetOffsetBySide(side)) - buttonSize * 0.5f, buttonSize),
                    text: $"+{side}");

                if (addNewNodeButtonClicked)
                {
                    Grid.AddNewNode(node.Index.IndexBySide(side));

                    Debug.Log($"Node added to {side}");

                    Grid.RefreshNodesPositions();
                    return true;
                }

                Handles.EndGUI();

                SceneView.RepaintAll();
            }

            return false;
        }
        #endregion

        #region Offsets
        private void DrawOffsetsHandles()
        {
            if (_changingOffsetsAllowed)
                return;

            EditorGUI.BeginChangeCheck();

            var nodesHorizontalOffsetTemp = Handles.FreeMoveHandle(Vector2.zero + Grid.NodesHorizontalOffset, Quaternion.identity, 0.2f, Vector2.zero, Handles.RectangleHandleCap);
            var nodesVerticalOffsetTemp = Handles.FreeMoveHandle(Vector2.zero + Grid.NodesVerticalOffset, Quaternion.identity, 0.2f, Vector2.zero, Handles.RectangleHandleCap);

            Handles.BeginGUI();

            Handles.EndGUI();

            if (EditorGUI.EndChangeCheck())
            {
                Grid.SetOffsets(nodesHorizontalOffsetTemp, nodesVerticalOffsetTemp);

                Debug.Log("Offsets was changed");
                Grid.RefreshNodesPositions();
            }

            Handles.color = Color.red;
            Handles.DrawLine(Grid.RootNode.Position, Grid.RootNode.Position + Grid.NodesHorizontalOffset);

            Handles.color = Color.green;
            Handles.DrawLine(Grid.RootNode.Position, Grid.RootNode.Position + Grid.NodesVerticalOffset);
        }

        #endregion
    }
}