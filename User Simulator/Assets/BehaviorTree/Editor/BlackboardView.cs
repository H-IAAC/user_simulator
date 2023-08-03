using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using System;
using System.Reflection;
using UnityEngine.UIElements;

public class BlackboardField2 : BlackboardField
{
    public Action OnPropertySelect;
    public Action<BlackboardField2> OnFieldDelete;

    public override void OnSelected()
    {
        OnPropertySelect();
    }

    protected override void ExecuteDefaultAction(EventBase evt)
    {
        base.ExecuteDefaultAction(evt);

        //Debug.Log($"{evt} | {(evt.target as BlackboardField2).text}");

        /*if(evt.eventTypeId == DragLeaveEvent.TypeId())
        {
            Debug.Log("DragLeaveEvent");
        }*/
    }

    
}

public class BlackboardView : Blackboard
{
    public Action<UnityEngine.Object> OnPropertySelect;

    BehaviorTree tree;

    public BlackboardView(GraphView associatedGraphView = null) : base(associatedGraphView)
    {
        SetPosition(new Rect(10, 30, 200, 300));
        scrollable = true;


        Add(new BlackboardSection { title = "Exposed Properties" });
    
        addItemRequested = _blackboard => { AddItemRequestHandler(); };
        
        editTextRequested = (blackboard, element, newText) =>
        {
            if(newText == "")
            {
                Debug.LogError("Name cannot be empty.");
                return;
            }

            BlackboardField field = element as BlackboardField;
            string oldName = field.text;

            if(tree.blackboard.Any(x => x.PropertyName == newText))
            {
                Debug.LogError("This name is already in use.");
                return;
            }

            int index = tree.blackboard.FindIndex(x => x.PropertyName == oldName);
            tree.blackboard[index].PropertyName = newText;
            field.text = newText;
        };


    }

    public void AddItemRequestHandler()
    {
        GenericMenu menu = new();

        TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom(typeof(BlackboardProperty));
        foreach(Type type in types)
        {
            if(type.IsAbstract)
            {
                continue;
            }

            menu.AddItem(new GUIContent($"{type.Name}"), false, () => CreateProperty(type));
        }

        menu.ShowAsContext();
    }

    public void CreateProperty(Type type)
    {
        if(tree == null)
        {
            Debug.LogError("Cannot create property without active tree asset.");
            return;
        }


        BlackboardProperty property = ScriptableObject.CreateInstance(type) as BlackboardProperty;

        string name = property.PropertyTypeName;
        while (tree.blackboard.Any( x => x.PropertyName == name))
        {
            name += "(1)";
        }
        property.PropertyName = name;

        tree.blackboard.Add(property);
        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(property, tree);
        }
        Undo.RegisterCreatedObjectUndo(property, "Behavior Tree (CreateProperty)");

        AssetDatabase.SaveAssets();

        drawProperty(property);
    }

    void drawProperty(BlackboardProperty property)
    {
        VisualElement container = new();
        
        BlackboardField2 field = new(){ text = property.PropertyName, typeText = $"{property.PropertyTypeName} property" };
        field.OnPropertySelect = () => { OnPropertySelect(property); };

        container.Add(field);
        Add(container);

        

    }

    public void PopulateView(BehaviorTree tree)
    {
        Clear();

        this.tree = tree;
        
        if(tree == null)
        {
            return;
        }

        if(tree.blackboard == null)
        {
            tree.blackboard = new();
        }

        foreach(BlackboardProperty property in tree.blackboard)
        {
            drawProperty(property);
        }
    }

    public void OnFieldDelete(BlackboardField field)
    {
        string name = field.text;

        int index = tree.blackboard.FindIndex(x => x.PropertyName == name);

        Debug.Log($"Removing {name} from {tree.name}");

        tree.blackboard.RemoveAt(index);
    }

}