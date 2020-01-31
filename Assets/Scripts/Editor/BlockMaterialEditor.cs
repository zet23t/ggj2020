using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BlockMaterial))]
public class BlockMaterialEditor : Editor {
    public override void OnInspectorGUI() {
        // EditorGUILayout.PropertyField()
       
        EditorGUILayout.PropertyField(serializedObject.FindProperty("MaterialPrefab"));
        serializedObject.ApplyModifiedProperties();
    }
}