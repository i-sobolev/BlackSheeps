using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavMeshGrid))]
public class NavMeshGridEditor : Editor
{
    private bool _changingOffsetsAllowed;

    private Vector3 _lastTargetPosition;

    protected NavMeshGrid Target => target as NavMeshGrid;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _changingOffsetsAllowed = EditorGUILayout.BeginToggleGroup("Nodes offsets handles", _changingOffsetsAllowed);
        EditorGUILayout.EndToggleGroup();
    }

    private void OnSceneGUI()
    {
        MoveRootNodeToTargetTransforPositins();

        DrawNodesConnections();
        DrawOffsetsHandles();
        DrawNodesIndexes();
        DrawNewNodesButtonsForAll();
    }

    private void MoveRootNodeToTargetTransforPositins()
    {
        var currentTargetPosition = Target.transform.position;

        if (_lastTargetPosition != currentTargetPosition)
        {
            _lastTargetPosition = currentTargetPosition;
            Target.RootNode.SetPosition(_lastTargetPosition);

            Target.RefreshNodesPositions();
        }
    }

    private void DrawNodesIndexes()
    {
        foreach (var node in Target.Nodes)
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

        foreach (var node in Target.Nodes)
        {
            foreach (var neighboringNode in node.AllNeighboringNodes)
            {
                Handles.DrawLine(node.Position, neighboringNode.Position, 1);
            }
        }
    }

    #region GridEdititg
    private void DrawNewNodesButtonsForAll()
    {
        foreach (var node in Target.Nodes)
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
                position: new Rect(HandleUtility.WorldToGUIPoint(node.Position + Target.GetOffsetBySide(side)) - buttonSize * 0.5f, buttonSize),
                text: $"+{side}");

            if (addNewNodeButtonClicked)
            {
                Target.AddNewNode(node.Index.IndexBySide(side));
                
                Debug.Log($"Node added to {side}");

                Target.RefreshNodesPositions();
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

        var nodesHorizontalOffsetTemp = Handles.FreeMoveHandle(Vector2.zero + Target.NodesHorizontalOffset, Quaternion.identity, 0.2f, Vector2.zero, Handles.RectangleHandleCap);
        var nodesVerticalOffsetTemp = Handles.FreeMoveHandle(Vector2.zero + Target.NodesVerticalOffset, Quaternion.identity, 0.2f, Vector2.zero, Handles.RectangleHandleCap);

        Handles.BeginGUI();

        Handles.EndGUI();

        if (EditorGUI.EndChangeCheck())
        {
            Target.SetOffsets(nodesHorizontalOffsetTemp, nodesVerticalOffsetTemp);

            Debug.Log("Offsets was changed");
            Target.RefreshNodesPositions();
        }

        Handles.color = Color.red;
        Handles.DrawLine(Target.RootNode.Position, Target.RootNode.Position + Target.NodesHorizontalOffset);

        Handles.color = Color.green;
        Handles.DrawLine(Target.RootNode.Position, Target.RootNode.Position + Target.NodesVerticalOffset);
    }

    #endregion
}