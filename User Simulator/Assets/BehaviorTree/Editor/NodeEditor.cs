using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;

[CustomEditor(typeof(Node), true)]
public class NodeEditor : Editor
{
    bool showProperties = true;

    static readonly string[] noDraw = new string[]{ 
        "useMemory",
        "useUtility", "utilitySelectionMethod", "utilityThreshould",
        "subtree"};

    static readonly string[] utilityProperties = new string[]{
        "utilitySelectionMethod", "utilityThreshould"
    };

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, noDraw);

        Node node = target as Node;
        SubtreeNode subtreeNode = node as SubtreeNode;
        RequestBehaviorNode requestBehaviorNode = node as RequestBehaviorNode;

        if(node.MemoryMode == MemoryMode.Both)
        {
            node.UseMemory = EditorGUILayout.Toggle("Use memory", node.UseMemory);
        }

        if(node is CompositeNode composite)
        {
            composite.useUtility = EditorGUILayout.Toggle("Use utility", composite.useUtility);

            if(composite.useUtility)
            {
                foreach(string propertyName in utilityProperties)
                {
                    if(propertyName == "utilityThreshould" &&
                        composite.utilitySelectionMethod != UtilitySelectionMethod.RANDOM_THRESHOULD)
                    {
                        continue;
                    }

                    SerializedProperty property = serializedObject.FindProperty(propertyName);
                    EditorGUILayout.PropertyField(property, true);
                }
            }
        }

        if(subtreeNode && !requestBehaviorNode)
        {
            SerializedProperty property = serializedObject.FindProperty("subtree");
            EditorGUILayout.PropertyField(property, true);
        }

        if (node.variables.Count > 0)
        {
            showProperties = EditorGUILayout.BeginFoldoutHeaderGroup(showProperties, "Properties");
            if (showProperties)
            {
                List<string> blackboardPropertiesList = new();
                blackboardPropertiesList.Add("None");

                foreach(BlackboardProperty property in node.tree.blackboard)
                {
                    blackboardPropertiesList.Add(property.PropertyName);
                }

                string[] blackboardProperties = blackboardPropertiesList.ToArray();

                for (int i = 0; i < node.propertyBlackboardMap.Count; i++)
                {
                    if(node.variables[i] == null)
                    {
                        Debug.LogWarning($"Property {i} of node {node.name} is null");
                        continue;
                    }

                    NameMap map = node.propertyBlackboardMap[i];

                    EditorGUILayout.LabelField(map.variable, EditorStyles.boldLabel);

                    EditorGUILayout.BeginHorizontal();

                    if (subtreeNode && !requestBehaviorNode)
                    {
                        float oldWidth2 = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 70;
                        subtreeNode.passValue[i] = EditorGUILayout.Toggle("Pass Value", subtreeNode.passValue[i]);
                        EditorGUIUtility.labelWidth = oldWidth2;
                    }

                    //float oldWidth = EditorGUIUtility.labelWidth;
                    //EditorGUIUtility.labelWidth = 110;
                    //string newValue = EditorGUILayout.TextField("Blackboard target", map.blackboardProperty);
                    //EditorGUIUtility.labelWidth = oldWidth;

                    int currentIndex = 0;
                    for(int j = 0; j<blackboardProperties.Length; j++)
                    {
                        string name = blackboardProperties[j];
                        if(map.blackboardProperty == name)
                        {
                            currentIndex = j;
                            break;
                        } 
                    }

                    float oldWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 110;
                    currentIndex = EditorGUILayout.Popup("Blackboard target", currentIndex, blackboardProperties);
                    EditorGUIUtility.labelWidth = oldWidth;

                    string newValue = "";

                    if(currentIndex != 0)
                    {
                        newValue = blackboardProperties[currentIndex];
                    }



                    node.propertyBlackboardMap[i] = new NameMap { variable = map.variable, blackboardProperty = newValue };
                    EditorGUILayout.EndHorizontal();

                    if (newValue == "")
                    {
                        Editor editor = CreateEditor(node.variables[i]);
                        editor.OnInspectorGUI();
                    }

                    EditorGUILayout.Space();
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            if(subtreeNode && !requestBehaviorNode)
            {
                if(GUILayout.Button("Autoremap"))
                {
                    subtreeNode.autoRemap();
                }
            }
        }


        serializedObject.ApplyModifiedProperties();
    }
}