using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;


namespace HIAAC.BehaviorTree
{
    [Serializable]
    public class BlackboardOverridableProperty
    {
        [SerializeReference] public BlackboardProperty property;
        public string parentName = "";

        public string Name
        {
            get
            {
                return property.PropertyName;
            }
            set
            {
                property.PropertyName = value;
            }
        }
    }

    [Serializable]
    public class Blackboard
    {
        [SerializeField] public List<BlackboardOverridableProperty> properties;

        public Blackboard parent = null;

        [HideInInspector] public bool runtime;

        UnityEngine.Object baseObject;

        public Blackboard(UnityEngine.Object baseObject, bool runtime)
        {
            properties = new();
            this.runtime = runtime;
            this.baseObject = baseObject;
        }

        public BlackboardProperty CreateProperty(Type type, string name)
        {
            while (properties.Any(x => x.Name == name))
            {
                name += "(1)";
            }

            BlackboardProperty property = BlackboardProperty.CreateInstance(type);

            property.PropertyName = name;

            BlackboardOverridableProperty op = new()
            {
                parentName = "",
                property = property
            };
            properties.Add(op);

            return property;
        }

        public BlackboardProperty CreateProperty(Type type)
        {
            BlackboardProperty property = BlackboardProperty.CreateInstance(type);
            string name = property.PropertyTypeName;

            return CreateProperty(type, name);
        }

        public BlackboardProperty GetProperty(string name, bool forceNodeProperty = false)
        {
            int index = properties.FindIndex(x => x.Name == name);

            if (index < 0)
            {
                throw new ArgumentException("Property does not exist in node.");
            }

            string parentName = properties[index].parentName;
            if (parentName == "" || forceNodeProperty)
            {
                return properties[index].property;
            }
            else
            {
                return parent.GetProperty(parentName);
            }
        }

        public object GetPropertyValue(string name, bool forceNodeProperty = false)
        {
            return GetProperty(name, forceNodeProperty).Value;
        }

        public T GetPropertyValue<T>(string name, bool forceNodeProperty = false)
        {
            return (T)GetProperty(name, forceNodeProperty).Value;
        }

        public void SetPropertyValue<T>(string name, T value, bool forceNodeProperty = false)
        {
            GetProperty(name, forceNodeProperty).Value = value;
        }

        public void ClearPropertyDefinitions(List<string> dontDelete = null)
        {

            if (dontDelete != null)
            {
                properties.RemoveAll(x => !dontDelete.Contains(x.Name));
            }
            else
            {
                properties.Clear();
            }

        }

        public bool HasProperty(string name)
        {
            return properties.Any(x => x.Name == name);
        }

         public void DeleteProperty(BlackboardProperty property)
        {
#if UNITY_EDITOR
            Undo.RecordObject(baseObject, "Behavior Tree (DeleteTreeProperty)");
#endif

            int index = properties.FindIndex(x => x.property == property);
            properties.RemoveAt(index);

#if UNITY_EDITOR
            AssetDatabase.SaveAssets();
#endif

        }

    }
}