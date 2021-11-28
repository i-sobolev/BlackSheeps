using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NavMeshGrid))]
public class NavMeshGridEditor : Editor
{
    private NavMeshGrid Target => target as NavMeshGrid;

    private Vector2 _nodesHorizontalOffset = Vector2.right;
    private Vector2 _nodesVerticalOffset = Vector2.up;

    private void OnSceneGUI()
    {
        _nodesHorizontalOffset = Handles.FreeMoveHandle(_nodesHorizontalOffset, Quaternion.identity, 0.2f, Vector2.zero, Handles.RectangleHandleCap);
        _nodesVerticalOffset = Handles.FreeMoveHandle(_nodesVerticalOffset, Quaternion.identity, 0.2f, Vector2.zero, Handles.RectangleHandleCap);

        Handles.color = Color.blue;

        foreach (var node in Target.Nodes)
        {
            if (node.TryGetNeighboringNodeBySide(NeighboringNodeSide.Left, out var leftNode))
                leftNode.SetPosition(node.Position + GetOffsetBySide(NeighboringNodeSide.Left));

            if (node.TryGetNeighboringNodeBySide(NeighboringNodeSide.Right, out var rightNode))
                rightNode.SetPosition(node.Position + GetOffsetBySide(NeighboringNodeSide.Right));

            if (node.TryGetNeighboringNodeBySide(NeighboringNodeSide.Upper, out var upperNode))
                upperNode.SetPosition(node.Position + GetOffsetBySide(NeighboringNodeSide.Upper));

            if (node.TryGetNeighboringNodeBySide(NeighboringNodeSide.Lower, out var lowerNode))
                lowerNode.SetPosition(node.Position + GetOffsetBySide(NeighboringNodeSide.Lower));
        }

        foreach (var node in Target.Nodes)
        {
            Handles.BeginGUI();

            GUI.TextField(
                position: new Rect(HandleUtility.WorldToGUIPoint(node.Position), new Vector2(60, 20)), 
                text: $"{node.Index.Row} {node.Index.Column}");

            Handles.EndGUI();

            foreach (var neighboringNode in node.AllNeighboringNodes)
                Handles.DrawLine(node.Position, neighboringNode.Position);

            foreach (var side in node.AllEmptyNeighboringNodesSides)
            {
                if (IsSideAreDiag(side))
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

                    Debug.Log($"Node added to {side}");
                    return;
                }

                Handles.EndGUI();

                SceneView.RepaintAll();
            }
        }

        Handles.color = Color.red;
        Handles.DrawLine(Target.RootNode.Position, _nodesHorizontalOffset);

        Handles.color = Color.green;
        Handles.DrawLine(Target.RootNode.Position, _nodesVerticalOffset);
    }

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

    private bool IsSideAreDiag(NeighboringNodeSide neighboringNodeSide)
    {
        var isUpperLeft = neighboringNodeSide == NeighboringNodeSide.UpperLeft;
        var isUpperRight = neighboringNodeSide == NeighboringNodeSide.UpperRight;
        var isLowerRight = neighboringNodeSide == NeighboringNodeSide.LowerRight;
        var isLowerLeft = neighboringNodeSide == NeighboringNodeSide.LowerLeft;

        return isUpperLeft || isUpperRight || isLowerRight || isLowerLeft;
    }
}