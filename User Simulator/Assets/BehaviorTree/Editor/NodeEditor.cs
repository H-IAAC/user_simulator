using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Node), true)]
public class NodeEditor : Editor
{
    bool showProperties = false;


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Node node = target as Node;

        if (node.variables.Count > 0)
        {
            showProperties = EditorGUILayout.BeginFoldoutHeaderGroup(showProperties, "Properties");
            if (showProperties)
            {
                for (int i = 0; i < node.propertyBlackboardMap.Count; i++)
                {
                    NameMap map = node.propertyBlackboardMap[i];

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(map.variable);
                    string newValue = EditorGUILayout.TextField(map.blackboardProperty);
                    node.propertyBlackboardMap[i] = new NameMap { variable = map.variable, blackboardProperty = newValue };
                    EditorGUILayout.EndHorizontal();

                    if (newValue == "")
                    {
                        Editor editor = CreateEditor(node.variables[i]);
                        editor.OnInspectorGUI();
                    }
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}