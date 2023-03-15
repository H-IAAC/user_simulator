using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.ScriptableList
{
    public abstract class ScriptableList<T> : ScriptableObject, IList<T>
    {
        public List<T> List = new List<T>();

        [SerializeField] bool notResetOnEnable = false;

        [SerializeProperty("Count")][SerializeField] int itemCount;

        T IList<T>.this[int index] { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        void OnEnable()
        {
            if(!notResetOnEnable)
            {
                Reset();
            }

            itemCount = List.Count;
        }

        void OnValidate()
        {
            this.itemCount = List.Count;
        }

        public void Reset()
        {
            List.Clear();
            itemCount = 0;
        }

        //Interface properties
        
        public int Count
        {
            get
            {
                return itemCount;
            }
        }

        public bool IsReadOnly => false;

        public T this[int index] 
        { 
            get
            {
                return List[index];
            } 
        }

        //Interface methods

        public void Add(T item)
        {
            if(!List.Contains(item))
            {
                List.Add(item);

                itemCount += 1;
            }
        }

        public bool Remove(T item)
        {
            if(List.Contains(item))
            {
                List.Remove(item);
                itemCount -= 1;
                
                return true;
            }

            return false;
        }

        public int IndexOf(T item)
        {
            return List.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if(!List.Contains(item))
            {
                List.Insert(index, item);
                itemCount += 1;   
            }
        }

        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
            itemCount -= 1;
        }

        public void Clear()
        {
            Reset();
        }

        public bool Contains(T item)
        {
            return List.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        public List<T>.Enumerator GetEnumerator()
        {
            return List.GetEnumerator();
        }

        //???

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}