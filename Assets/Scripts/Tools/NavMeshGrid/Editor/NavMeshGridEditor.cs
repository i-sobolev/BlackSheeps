using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavMeshGrid))]
public partial class NavMeshGridEditor : Editor
{
    protected NavMeshGrid Target => target as NavMeshGrid;

    private Vector2 _nodesHorizontalOffset = Vector2.right;
    private Vector2 _nodesVerticalOffset = Vector2.up;

    private void OnSceneGUI()
    {
        Target.RootNode.SetPosition(Target.transform.position);

        DrawNodesConnections();

        DrawOffsetsHandles();

        foreach (var node in Target.Nodes)
        {
            Handles.BeginGUI();

            GUI.Label(
                position: new Rect(HandleUtility.WorldToGUIPoint(node.Position), new Vector2(60, 20)),
                text: $"{node.Index.Row} {node.Index.Column}");

            Handles.EndGUI();


            foreach (var side in node.AllEmptyNeighboringNodesSides)
            {
                if (IsSideAreDiagonal(side))
                    continue;

                Handles.BeginGUI();

                var buttonSize = new Vector2(50f, 20f);

                var addNewNodeButtonClicked = GUI.Button(
                    position: new Rect(HandleUtility.WorldToGUIPoint(node.Position + GetOffsetBySide(side)) - buttonSize * 0.5f, buttonSize),
                    text: $"+{side}");

                if (addNewNodeButtonClicked)
                {
                    var newNodeIndex = Index.NewIndexBySide(side, node.Index);

                    Target.AddNewNode(newNodeIndex);

                    EditorUtility.SetDirty(Target);

                    Debug.Log($"Node added to {side}");

                    RefreshNodesPositions();
                    return;
                }

                Handles.EndGUI();

                SceneView.RepaintAll();
            }
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

    #region Offsets
    private void DrawOffsetsHandles()
    {
        EditorGUI.BeginChangeCheck();

        var nodesHorizontalOffsetTemp = Handles.FreeMoveHandle(Vector2.zero + _nodesHorizontalOffset, Quaternion.identity, 0.2f, Vector2.zero, Handles.RectangleHandleCap);
        var nodesVerticalOffsetTemp = Handles.FreeMoveHandle(Vector2.zero + _nodesVerticalOffset, Quaternion.identity, 0.2f, Vector2.zero, Handles.RectangleHandleCap);

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
        void TryGetNodeAndSetPositionBySide(NavMeshGridNode node, NeighboringNodeSide neededNodeSide)
        {
            if (node.TryGetNeighboringNodeBySide(neededNodeSide, out var neededNode))
                neededNode.SetPosition(node.Position + GetOffsetBySide(neededNodeSide));
        }

        foreach (var node in Target.Nodes)
        {
            TryGetNodeAndSetPositionBySide(node, NeighboringNodeSide.Left);
            TryGetNodeAndSetPositionBySide(node, NeighboringNodeSide.Right);
            TryGetNodeAndSetPositionBySide(node, NeighboringNodeSide.Upper);
            TryGetNodeAndSetPositionBySide(node, NeighboringNodeSide.Lower);
        }
    }
    #endregion

    #region HelpFunctions
    private Vector2 GetOffsetBySide(NeighboringNodeSide neighboringNodeSide)
    {
        return neighboringNodeSide switch
        {
            NeighboringNodeSide.Left => -_nodesHorizontalOffset,
            NeighboringNodeSide.Right => _nodesHorizontalOffset,
            NeighboringNodeSide.Lower => -_nodesVerticalOffset,
            NeighboringNodeSide.Upper => _nodesVerticalOffset,
            _ => Vector2.zero,
        };
    }
    private bool IsSideAreDiagonal(NeighboringNodeSide neighboringNodeSide)
    {
        var isUpperLeft = neighboringNodeSide == NeighboringNodeSide.UpperLeft;
        var isUpperRight = neighboringNodeSide == NeighboringNodeSide.UpperRight;
        var isLowerRight = neighboringNodeSide == NeighboringNodeSide.LowerRight;
        var isLowerLeft = neighboringNodeSide == NeighboringNodeSide.LowerLeft;

        return isUpperLeft || isUpperRight || isLowerRight || isLowerLeft;
    }
    #endregion
}