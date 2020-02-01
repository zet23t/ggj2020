using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Block))]
public class BlockEditor : Editor {
    static BlockOrientation blockOrientation;
    public override void OnInspectorGUI() {
        // EditorGUILayout.PropertyField()
        SerializedProperty widthProperty = serializedObject.FindProperty("width");
        SerializedProperty heightProperty = serializedObject.FindProperty("height");
        EditorGUILayout.PropertyField(widthProperty);
        EditorGUILayout.PropertyField(heightProperty);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Prefab"));
        int width = Mathf.Clamp(widthProperty.intValue, 1, 8);
        int height = Mathf.Clamp(heightProperty.intValue, 1, 8);
        if (width != widthProperty.intValue) widthProperty.intValue = width;
        if (height != heightProperty.intValue) heightProperty.intValue = height;
        SerializedProperty fieldsProperty = serializedObject.FindProperty("fields");
        fieldsProperty.arraySize = width * height;
        EditorGUI.indentLevel += 1;
        for (int y = 0; y < height; y += 1)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Line "+y);
            for (int x = 0; x < width; x += 1)
            {
                var element = fieldsProperty.GetArrayElementAtIndex(y * width + x);
                var b = EditorGUILayout.Toggle(element.boolValue, GUILayout.Width(30));
                element.boolValue = b;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel -= 1;
        serializedObject.ApplyModifiedProperties();

        blockOrientation = (BlockOrientation) EditorGUILayout.EnumPopup("Orientation preview", blockOrientation);
        Block rotblock = (target as Block).Clone();
        rotblock.Rotate(blockOrientation);
        GUI.enabled = false;
        width = rotblock.Width;
        height = rotblock.Height;
        for (int y = 0; y < height; y += 1)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Line "+y);
            for (int x = 0; x < width; x += 1)
            {
                EditorGUILayout.Toggle(rotblock.IsFieldSet(x, y), GUILayout.Width(30));
            }
            EditorGUILayout.EndHorizontal();
        }
        GUI.enabled = true;
    }
}