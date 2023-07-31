using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using System;
using System.Reflection;
using UnityEngine.UIElements;

class BlackboardView : Blackboard
{
    public List<BlackboardProperty> properties = new();

    public BlackboardView(GraphView associatedGraphView = null) : base(associatedGraphView)
    {
        Add(new BlackboardSection { title = "Exposed Properties" });
        addItemRequested = _blackboard => { AddItemRequestHandler(); };
        SetPosition(new Rect(10, 30, 200, 300));
    }

    public void AddItemRequestHandler()
    {
        GenericMenu menu = new();

        TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom(typeof(BlackboardProperty));
        foreach(Type type in types)
        {
            menu.AddItem(new GUIContent($"{type.Name}"), false, () => CreateProperty(type));
        }

        menu.ShowAsContext();
    }

    public void CreateProperty(Type type)
    {
        BlackboardProperty property = Activator.CreateInstance(type) as BlackboardProperty;
        properties.Add(property);

        VisualElement container = new();
        BlackboardField field = new(){ text = property.PropertyName, typeText = $"{property.PropertyTypeName} property" };
        container.Add(field);
        Add(container);

    }
}