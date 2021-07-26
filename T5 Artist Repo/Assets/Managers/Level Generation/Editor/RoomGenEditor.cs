using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RoomScript))]
public class RoomGenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RoomScript script = (RoomScript) target;

        if (script.isEntrance)
        {
            EditorUtility.SetDirty(script);
            script.spawnPoint = (Transform) EditorGUILayout.ObjectField("Spawn Point", script.spawnPoint, typeof(Transform), true);
            script.playerReference = (GameObject) EditorGUILayout.ObjectField("Player Reference", script.playerReference, typeof(GameObject), true);
        }
        else
            EditorUtility.ClearDirty(script);

    }
}
