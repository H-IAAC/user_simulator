using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using System;
using System.Reflection;
using UnityEngine.UIElements;

namespace HIAAC.BehaviorTree
{

    public class BlackboardView : Blackboard
    {
        public Action<UnityEngine.Object> OnPropertySelect; //Action to execute on field selection

        BehaviorTree tree; //Active behavior tree

        /// <summary>
        /// View constructor.
        /// </summary>
        /// <param name="associatedGraphView">Associated BT View.</param>
        public BlackboardView(BehaviorTreeView associatedGraphView = null) : base(associatedGraphView)
        {
            SetPosition(new Rect(10, 30, 200, 300));
            scrollable = true;
            Add(new BlackboardSection { title = "Exposed Properties" });

            //Configure actions
            addItemRequested = _blackboard => AddItemRequestHandler();
            editTextRequested = (blackboard, element, newText) => EditItemRequestHandler(element, newText);
        }

        /// <summary>
        /// Draw property on the view.
        /// </summary>
        /// <param name="property">Property to draw.</param>
        void drawProperty(BlackboardProperty property)
        {
            VisualElement container = new();

            BlackboardField2 field = new()
            {
                text = property.PropertyName,
                typeText = $"{property.PropertyTypeName} property",
                OnPropertySelect = () => { OnPropertySelect(property); }
            };

            container.Add(field);
            Add(container);

        }

        /// <summary>
        /// Create new property.
        /// </summary>
        /// <param name="type">Property type. Must inherit from BlackboardProperty.</param>
        public void CreateProperty(Type type)
        {
            //Check if have active tree
            if (tree == null)
            {
                Debug.LogError("Cannot create property without active tree asset.");
                return;
            }

            BlackboardProperty property = ScriptableObject.CreateInstance(type) as BlackboardProperty;

            //Ensures name isn't duplicated
            string name = property.PropertyTypeName;
            while (tree.blackboard.Any(x => x.PropertyName == name))
            {
                name += "(1)";
            }
            property.PropertyName = name;

            //Add property to tree.
            tree.blackboard.Add(property);
            if (!Application.isPlaying)
            {
                AssetDatabase.AddObjectToAsset(property, tree);
            }
            Undo.RegisterCreatedObjectUndo(property, "Behavior Tree (CreateProperty)");
            AssetDatabase.SaveAssets();

            //Draw property on the view.
            drawProperty(property);
        }

        /// <summary>
        /// Delete an property.
        /// </summary>
        /// <param name="field">Property to delete.</param>
        public void DeleteProperty(BlackboardField field)
        {   
            //Get property index on blackboard
            string name = field.text;
            int index = tree.blackboard.FindIndex(x => x.PropertyName == name);

            //Remove property
            tree.DeleteProperty(tree.blackboard[index]);
        }

        /// <summary>
        /// Populate view with new behavior tree.
        /// </summary>
        /// <param name="tree">Behavior Tree to populate view.</param>
        public void PopulateView(BehaviorTree tree)
        {
            //Clear previous tree properties.
            Clear();

            this.tree = tree;
            if (tree == null)
            {
                return;
            }

            //Draw tree properties
            foreach (BlackboardProperty property in tree.blackboard)
            {
                drawProperty(property);
            }
        }

        // Event handlers // ----------------------------------------------------------------------------- //

        public void AddItemRequestHandler()
        {
            GenericMenu menu = new();

            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom(typeof(BlackboardProperty));
            foreach (Type type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                menu.AddItem(new GUIContent($"{type.Name}"), false, () => CreateProperty(type));
            }

            menu.ShowAsContext();
        }

        public void EditItemRequestHandler(VisualElement element, string newText)
        {
            if (newText == "")
            {
                Debug.LogError("Name cannot be empty.");
                return;
            }

            BlackboardField field = element as BlackboardField;
            string oldName = field.text;

            if (tree.blackboard.Any(x => x.PropertyName == newText))
            {
                Debug.LogError("This name is already in use.");
                return;
            }

            int index = tree.blackboard.FindIndex(x => x.PropertyName == oldName);
            tree.blackboard[index].PropertyName = newText;
            field.text = newText;
        }


    }
}