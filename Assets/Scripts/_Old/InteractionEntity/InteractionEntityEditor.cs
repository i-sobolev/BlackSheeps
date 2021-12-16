#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InteractionEntity), true)]
public class InteractionEntityEditor : Editor
{
    public void OnSceneGUI()
    {
        Handles.color = Color.yellow;

        InteractionEntity interactionEntity = (InteractionEntity)target;
        
        interactionEntity.InteractPoint = Handles.FreeMoveHandle
            (
                (Vector2)interactionEntity.transform.position + interactionEntity.InteractPoint, Quaternion.identity,
                0.2f * HandleUtility.GetHandleSize(interactionEntity.transform.position),
                Vector3.zero,
                Handles.ConeHandleCap
            ) - interactionEntity.transform.position;

        if (GUI.changed)
        {
            Undo.RecordObject(target, "Updated Sorting Offset");
            EditorUtility.SetDirty(target);
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}
#endif