using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.ScriptableList
{
    /// <summary>
    /// Serializable list for storing values.
    /// </summary>
    /// <typeparam name="T">Type of the list values</typeparam>
    public abstract class ScriptableList<T> : ScriptableObject, IList<T>
    {
        /// <summary>
        /// List values.
        /// </summary>
        [Tooltip("List values.")]
        public List<T> List = new List<T>();

        [Tooltip("If should reset the list values on enable.")]
        [SerializeField] bool notResetOnEnable = false;

        [Tooltip("List item count. DO NOT CHANGE")]
        [SerializeProperty("Count")][SerializeField] int itemCount;

        T IList<T>.this[int index] { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        /// <summary>
        /// Resets the list if enabled. Updates the item count.
        /// </summary>
        void OnEnable()
        {
            if(!notResetOnEnable)
            {
                Reset();
            }

            itemCount = List.Count;
        }

        /// <summary>
        /// Updates the item count.
        /// </summary>
        void OnValidate()
        {
            this.itemCount = List.Count;
        }


        /// <summary>
        /// Resets the list, clearing it values
        /// </summary>
        public void Reset()
        {
            List.Clear();
            itemCount = 0;
        }

        //Interface properties
        
        /// <summary>
        /// Number of elements in the list.
        /// </summary>
        public int Count
        {
            get
            {
                return itemCount;
            }
        }

        public bool IsReadOnly => false;

        /// <summary>
        /// Getter for elements in the list.
        /// </summary>
        /// <param name="index">Index of the element in the list.</param>
        /// <returns>Element at index</returns>
        public T this[int index] 
        { 
            get
            {
                return List[index];
            } 
        }

        //Interface methods

        /// <summary>
        /// Adds an element to the list.
        /// </summary>
        /// <param name="item">Element to add</param>
        public void Add(T item)
        {
            if(!List.Contains(item))
            {
                List.Add(item);

                itemCount += 1;
            }
        }

        /// <summary>
        /// Removes an element to the list.
        /// </summary>
        /// <param name="item">Element to remove</param>
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


        /// <summary>
        /// Gets the index of an element in the list.
        /// </summary>
        /// <param name="item">Element to get the index</param>
        /// <returns>Index of the element</returns>
        public int IndexOf(T item)
        {
            return List.IndexOf(item);
        }

        /// <summary>
        /// Inserts an element to the list in a given index.
        /// </summary>
        /// <param name="index">Index to insert the element.</param>
        /// <param name="item">Element to insert</param>
        public void Insert(int index, T item)
        {
            if(!List.Contains(item))
            {
                List.Insert(index, item);
                itemCount += 1;   
            }
        }

        /// <summary>
        /// Removes the element at the index.
        /// </summary>
        /// <param name="index">Index to remove the element</param>
        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
            itemCount -= 1;
        }

        /// <summary>
        /// Clear the list.
        /// </summary>
        public void Clear()
        {
            Reset();
        }

        /// <summary>
        /// Checks if element is in the list
        /// </summary>
        /// <param name="item">Element to check</param>
        /// <returns>True if item is in the list, false otherwise.</returns>
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