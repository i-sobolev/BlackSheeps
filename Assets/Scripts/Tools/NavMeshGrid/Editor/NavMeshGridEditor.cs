using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavMeshGrid))]
public partial class NavMeshGridEditor : Editor
{
    protected NavMeshGrid Target => target as NavMeshGrid;

    private Vector2 _nodesHorizontalOffset = Vector2.right;
    private Vector2 _nodesVerticalOffset = Vector2.up;

    private Vector3 _lastTargetPosition;

    private bool _changingOffsetsAllowed;

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

            RefreshNodesPositions();
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
            DrawNewNodesButtons(node);
    }

    private void DrawNewNodesButtons(NavMeshGridNode node)
    {
        foreach (var side in node.AllEmptyNeighboringNodesSides)
        {
            if (side.IsDiagonal())
                continue;

            Handles.BeginGUI();

            var buttonSize = new Vector2(50f, 20f);

            var addNewNodeButtonClicked = GUI.Button(
                position: new Rect(HandleUtility.WorldToGUIPoint(node.Position + GetOffsetBySide(side)) - buttonSize * 0.5f, buttonSize),
                text: $"+{side}");

            if (addNewNodeButtonClicked)
            {
                Target.AddNewNode(node.Index.IndexBySide(side));

                EditorUtility.SetDirty(Target);

                Debug.Log($"Node added to {side}");

                RefreshNodesPositions();
                return;
            }

            Handles.EndGUI();

            SceneView.RepaintAll();
        }
    }
    #endregion

    #region Offsets
    private void DrawOffsetsHandles()
    {
        if (_changingOffsetsAllowed)
            return;

        EditorGUI.BeginChangeCheck();

        var nodesHorizontalOffsetTemp = Handles.FreeMoveHandle(Vector2.zero + _nodesHorizontalOffset, Quaternion.identity, 0.2f, Vector2.zero, Handles.RectangleHandleCap);
        var nodesVerticalOffsetTemp = Handles.FreeMoveHandle(Vector2.zero + _nodesVerticalOffset, Quaternion.identity, 0.2f, Vector2.zero, Handles.RectangleHandleCap);

        Handles.BeginGUI();

        Handles.EndGUI();

        if (EditorGUI.EndChangeCheck())
        {
            _nodesHorizontalOffset = nodesHorizontalOffsetTemp;
            _nodesVerticalOffset = nodesVerticalOffsetTemp;

            Debug.Log("Offsets was changed");
            RefreshNodesPositions();
        }

        Handles.color = Color.red;
        Handles.DrawLine(Target.RootNode.Position, Target.RootNode.Position + _nodesHorizontalOffset);

        Handles.color = Color.green;
        Handles.DrawLine(Target.RootNode.Position, Target.RootNode.Position + _nodesVerticalOffset);
    }

    private void RefreshNodesPositions()
    {
        void TryGetNodeAndSetPositionBySide(NavMeshGridNode node, Side neededNodeSide)
        {
            if (node.TryGetNeighboringNodeBySide(neededNodeSide, out var neededNode))
                neededNode.SetPosition(node.Position + GetOffsetBySide(neededNodeSide));
        }

        foreach (var node in Target.Nodes)
        {
            TryGetNodeAndSetPositionBySide(node, Side.Left);
            TryGetNodeAndSetPositionBySide(node, Side.Right);
            TryGetNodeAndSetPositionBySide(node, Side.Upper);
            TryGetNodeAndSetPositionBySide(node, Side.Lower);
        }
    }
    #endregion

    #region HelpFunctions
    private Vector2 GetOffsetBySide(Side neighboringNodeSide)
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
    #endregion
}