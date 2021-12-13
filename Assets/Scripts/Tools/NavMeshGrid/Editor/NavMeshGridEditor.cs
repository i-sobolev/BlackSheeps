using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NavMeshGrid
{
    [CustomEditor(typeof(NavMeshGrid))]
    public class NavMeshGridEditor : Editor
    {
        private Vector3 _lastTargetPosition;
        private string _loadingBackupName;

        private NavMeshGridNode _selectedNode = null;

        private bool _nodesListShowed = false;
        private bool _showGridOffsetsHandles = false;
        private bool _showNodesCustomOffsetsHandles = false;

        private readonly Color _nodesConnectionsColor = new Color(1, 1, 1, 0.1f);
        
        protected NavMeshGrid Grid => target as NavMeshGrid;
        
        public override void OnInspectorGUI()
        {
            ShowNodesArray(serializedObject.FindProperty("_nodes"), "Nodes list");

            void ShowNodesArray(SerializedProperty list, string label)
            {
                if (EditorGUILayout.DropdownButton(new GUIContent(label), FocusType.Passive))
                    _nodesListShowed = !_nodesListShowed;
                
                if (_nodesListShowed)
                {
                    for (int i = 0; i < list.arraySize; i++)
                    {
                        EditorGUI.indentLevel += 1;

                        var element = list.GetArrayElementAtIndex(i);

                        var node = element.objectReferenceValue as NavMeshGridNode;

                        EditorGUILayout.PropertyField(element,
                        new GUIContent($"Node \t{node.Index.Row}\t{node.Index.Column}"));
                        EditorGUI.indentLevel -= 1;
                    }
                }

                if (_selectedNode)
                {
                    for (int i = 0; i < list.arraySize; i++)
                    {
                        var element = list.GetArrayElementAtIndex(i);
                        var node = element.objectReferenceValue as NavMeshGridNode;

                        if (node.Index == _selectedNode.Index)
                        {
                            EditorGUILayout.Space(20);
                            GUILayout.Label("Selected node:");

                            EditorGUILayout.PropertyField(element,
                            new GUIContent($"Node \t{node.Index.Row}\t{node.Index.Column}"));
                            EditorGUI.indentLevel -= 1;
                            EditorGUILayout.Space();

                            var resetCustomOffsetButtonClicked = GUILayout.Button("Reset custom offset");

                            if (resetCustomOffsetButtonClicked)
                            {
                                _selectedNode.SetCustomOffset(Vector2.zero, 1);
                                Grid.RefreshNodesPositions();
                            }

                            Repaint();
                            break;
                        }
                    }
                }
            }
        }

        public void OnSceneGUI()
        {
            MoveRootNodeToTargetTransforPositins();

            DrawNodesConnections();
            DrawOffsetsHandles();
            DrawNodesIndexes();

            DrawNodesSelectButtons();

            DrawAgentsHandles();

            DrawSettingsWindow();
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

        private void DrawNodesSelectButtons()
        {
            Handles.color = Color.green;

            foreach (var node in Grid.Nodes)
            {
                if (node == Grid.RootNode)
                    continue;

                var nodeButtonClicked = Handles.Button(node.Position, Quaternion.identity, 0.2f, 0.2f, Handles.RectangleHandleCap);

                if (nodeButtonClicked)
                    _selectedNode = node;
            }


            if (_selectedNode)
            {
                DrawNewNodesButtons(_selectedNode);
                DrawNodeCustomOffsetHandle(_selectedNode);

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

        #region SettingsWindow
        private void DrawSettingsWindow()
        {
            Handles.BeginGUI();
            {
                GUILayout.BeginArea(new Rect(Vector2.one * 10f, new Vector2(200f, 110f)), new GUIStyle("box"));
                {
                    DrawBackupButtons();
                }
                GUILayout.EndArea();
            }
            Handles.EndGUI();

            Handles.BeginGUI();
            {
                GUILayout.BeginArea(new Rect(Vector2.one * 10f - Vector2.down * 130f, new Vector2(200f, 60f)), new GUIStyle("box"));
                {
                    GUILayout.Label("Toggle handles");
                    _showGridOffsetsHandles = GUILayout.Toggle(_showGridOffsetsHandles, "Grid offsets handles");
                    _showNodesCustomOffsetsHandles = GUILayout.Toggle(_showNodesCustomOffsetsHandles, "Nodes custom offsets handles");
                }
                GUILayout.EndArea();
            }
            Handles.EndGUI();
        }

        private void DrawBackupButtons()
        {
            var madeBackupButtonClicked = GUILayout.Button("Made backup");
            {
                if (madeBackupButtonClicked)
                    NavMeshGridBackupHelper.MadeBackup(Grid.Nodes.ToArray());
            }

            GUILayout.Space(20f);

            GUILayout.Label("Backup file name");
            {
                _loadingBackupName = GUILayout.TextField(_loadingBackupName);

                var LoadFromBackupButtonClicked = GUILayout.Button("Load from backup");

                if (LoadFromBackupButtonClicked)
                {
                    if (NavMeshGridBackupHelper.LoadModelFromFile(_loadingBackupName, out var nodes))
                        Grid.SetDataFromModel(nodes);
                }
            }
        }

        #endregion

        #region AgentsGridLinking
        public void DrawAgentsHandles()
        {
            Handles.color = Color.blue;

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
                    agent.LinkToGridNode(node, true);
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
                    position: new Rect(HandleUtility.WorldToGUIPoint(node.PositionWithoutOffset + Grid.GetOffsetBySide(side)) - buttonSize * 0.5f, buttonSize),
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
        private void DrawNodeCustomOffsetHandle(NavMeshGridNode node)
        {
            if (!_showNodesCustomOffsetsHandles)
                return;

            EditorGUI.BeginChangeCheck();

            Handles.color = Color.white;

            var buttonOffset = Vector2.down * 0.4f;
            var customOffset = Handles.FreeMoveHandle(node.Position + buttonOffset, Quaternion.identity, 0.1f, Vector3.zero, Handles.DotHandleCap);

            if (EditorGUI.EndChangeCheck())
            {
                node.SetCustomOffset((Vector2)customOffset - node.PositionWithoutOffset - buttonOffset, (Grid.NodesHorizontalOffset + Grid.NodesVerticalOffset).magnitude / 4f);
                Debug.Log(node.PositionWithoutOffset);
            }
        }

        private void DrawOffsetsHandles()
        {
            if (!_showGridOffsetsHandles)
                return;

            var rootNodePosition = Grid.RootNode.Position;
            var handleOffset = Vector2.right * 0.4f;

            EditorGUI.BeginChangeCheck();

            Handles.color = Color.red;
            var nodesHorizontalOffset = Handles.FreeMoveHandle(rootNodePosition + Grid.NodesHorizontalOffset + handleOffset, Quaternion.identity, 0.1f, Vector2.zero, Handles.DotHandleCap);
            
            Handles.color = Color.green;
            var nodesVerticalOffset = Handles.FreeMoveHandle(rootNodePosition + Grid.NodesVerticalOffset + handleOffset, Quaternion.identity, 0.1f, Vector2.zero, Handles.DotHandleCap);

            if (EditorGUI.EndChangeCheck())
            {
                Grid.SetOffsets(
                    (Vector2)nodesHorizontalOffset - rootNodePosition - handleOffset, 
                    (Vector2)nodesVerticalOffset - rootNodePosition - handleOffset);

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