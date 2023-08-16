using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

namespace HIAAC.BehaviorTree
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

        Editor editor;

        public InspectorView()
        {

        }

        public void UpdateSelection(UnityEngine.Object obj)
        {
            Clear();

            UnityEngine.Object.DestroyImmediate(editor);

            editor = Editor.CreateEditor(obj);
            IMGUIContainer container = new IMGUIContainer(OnGUIHandler);
            Add(container);
        }

        public void UpdateSelection(VisualElement container)
        {
            Clear();
            Add(container);
        }

        public void UpdateSelection(SerializedProperty property)
        {
            Clear();

            IMGUIContainer container = new()
            {
                onGUIHandler = () =>
                {
                    if (property.serializedObject.targetObject == null)
                    {
                        this.Clear();
                    }
                    else
                    {
                        property.serializedObject.Update();
                        EditorGUILayout.PropertyField(property, true);
                        property.serializedObject.ApplyModifiedProperties();
                    }


                }
            };

            Add(container);
        }

        void OnGUIHandler()
        {
            if (!editor)
            {
                return;
            }

            if (editor.target)
            {
                editor.OnInspectorGUI();
            }
        }
    }
}