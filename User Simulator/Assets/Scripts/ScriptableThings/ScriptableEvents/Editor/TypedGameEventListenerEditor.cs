using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(TypedGameEventListener<>), true)]
public class TypedGameEventListenerEditor : Editor
{
    List<SerializedProperty> properties;
    string[] hiddenProperties = new string[]{"Response"};

    public override void OnInspectorGUI()
    {
        DrawPropertiesExcluding(serializedObject, hiddenProperties);
        
        serializedObject.ApplyModifiedProperties();
		 
    }
   
}