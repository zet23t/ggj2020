using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Block))]
public class BlockEditor : Editor {
    public override void OnInspectorGUI() {
        // EditorGUILayout.PropertyField()
        SerializedProperty widthProperty = serializedObject.FindProperty("width");
        SerializedProperty heightProperty = serializedObject.FindProperty("height");
        EditorGUILayout.PropertyField(widthProperty);
        EditorGUILayout.PropertyField(heightProperty);
        int width = widthProperty.intValue;
        int height = heightProperty.intValue;
        serializedObject.FindProperty("fields").arraySize = width * height;
        for (int y = 0; y < height; y += 1)
        {

        }
    }
}