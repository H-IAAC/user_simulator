using System;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEditor;

namespace HIAAC.BehaviorTree
{
    [Serializable]
    public abstract class BlackboardProperty
    {
        public string PropertyName;

        public abstract string PropertyTypeName { get; }

        public abstract object Value { get; set; }

        public virtual BlackboardProperty Clone()
        {
            BlackboardProperty clone = CreateInstance(this.GetType());
            clone.PropertyName = PropertyName;
            clone.Value = Value;

            return clone;
        }
        

        public static BlackboardProperty CreateInstance<T>()
        {
            return Activator.CreateInstance(typeof(T)) as BlackboardProperty;
        }

        public static BlackboardProperty CreateInstance(Type type)
        {
            return Activator.CreateInstance(type) as BlackboardProperty;
        }

        public virtual void Validate()
        {

        }

    }

    [Serializable]
    public abstract class BlackboardProperty<T> : BlackboardProperty
    {
        [SerializeField]
        public T value = default;

        public override object Value
        {
            get { return value; }
            set { this.value = (T)value; }
        }

        public override string PropertyTypeName
        {
            get
            {
                string name = typeof(T).Name;

                string sufix = "Property";
                if (name.EndsWith(sufix))
                {
                    name = name.Substring(0, name.Length - sufix.Length);
                }

                return name;
            }
        }

    }
}
