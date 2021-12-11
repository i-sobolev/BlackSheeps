#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitSpawner))]
public class UnitSpawnerEditor : Editor
{
    public void OnSceneGUI()
    {
        if (Application.isPlaying)
            return;

        Handles.color = Color.blue;

        var spawner = (UnitSpawner)target;

        spawner.StartSpawnPoint = Handles.FreeMoveHandle
            (
                (Vector2)spawner.transform.position + spawner.StartSpawnPoint, Quaternion.identity,
                0.2f * HandleUtility.GetHandleSize(spawner.transform.position),
                Vector3.zero,
                Handles.ConeHandleCap
            ) - spawner.transform.position;

        spawner.EndSpawnPoint = Handles.FreeMoveHandle
            (
                (Vector2)spawner.transform.position + spawner.EndSpawnPoint, Quaternion.identity,
                0.2f * HandleUtility.GetHandleSize(spawner.transform.position),
                Vector3.zero,
                Handles.ConeHandleCap
            ) - spawner.transform.position;

        Handles.DrawLine((Vector2)spawner.transform.position + spawner.StartSpawnPoint, (Vector2)spawner.transform.position + spawner.EndSpawnPoint);

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