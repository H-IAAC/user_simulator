using System;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;


public abstract class BlackboardProperty : ScriptableObject
{
    public string PropertyName
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }

    public abstract string PropertyTypeName{ get; }

    public abstract object Value{ get; set; }

    public virtual BlackboardProperty Clone()
    {
        BlackboardProperty clone = CreateInstance(this.GetType()) as BlackboardProperty;
        clone.PropertyName = this.PropertyName;
        clone.Value = Value;

        return clone;
    }

}

public abstract class BlackboardProperty<T> : BlackboardProperty
{
    [SerializeField]
    public T value = default;

    public override object Value
    {
        get { return value; }
        set { this.value = (T) value; }
    }


    /*public override VisualElement CreateField()
    {
        IMGUIContainer container = new()
        {
            onGUIHandler = () =>
        {
            if(thisSerialized == null)
            {
                thisSerialized = new SerializedObject(this);
            }

            thisSerialized.Update();

            SerializedProperty b = thisSerialized.FindProperty("value");
            EditorGUILayout.PropertyField(b, true);
            
            thisSerialized.ApplyModifiedProperties();
        }
        };


        return container;
    }*/

    public override string PropertyTypeName  
    { 
        get 
        {
            string name = typeof(T).Name;

            string sufix = "Property";
            if(name.EndsWith(sufix))
            {
                name = name.Substring(0, name.Length - sufix.Length);
            }

            return name;  
        } 
    }

}
