using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTreeView : GraphView
{
    public Action<NodeView> OnNodeSelected;

    public new class UxmlFactory : UxmlFactory<BehaviorTreeView, GraphView.UxmlTraits>{}

    public BehaviorTree tree;

    public BlackboardView blackboard;

    List<GraphElement> clipboard;

    public BehaviorTreeView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        string[] guids = AssetDatabase.FindAssets("t:StyleSheet BehaviorTreeEditor");
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(guids[0]));
        styleSheets.Add(styleSheet);

        Undo.undoRedoPerformed += OnUndoRedo;

        clipboard = new();
        serializeGraphElements += OnCopyCut;
        unserializeAndPaste += OnPaste;
    }

    string OnCopyCut(IEnumerable<GraphElement> elements)
    {
        clipboard.Clear();
        clipboard.AddRange(elements);
        return "";
    }

    void OnPaste(string operationName, string data)
    {
        if(operationName == "Duplicate")
        {
            ClearSelection();

            Dictionary<Node, Node> originalToClone = new();

            foreach(GraphElement element in clipboard)
            {
                if(element is NodeView view)
                {
                    Node node = view.node;
                    Node clone = tree.DuplicateNode(node);

                    clone.position.x += 10;

                    NodeView cloneView = CreateNodeView(clone);

                    AddToSelection(cloneView);

                    originalToClone.Add(node, clone);
                }
            }

            //Create connections if duplicating multiple nodes and originals was connected
            foreach(Node node in originalToClone.Keys)
            {
                if(node.parent == null)
                {
                    continue;
                }

                if(originalToClone.ContainsKey(node.parent))
                {
                    Node parentClone = originalToClone[node.parent];
                    Node clone = originalToClone[node];

                    parentClone.AddChild(clone);

                    NodeView parentView = FindNodeView(parentClone);
                    NodeView childView = FindNodeView(clone);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                }
            }
        }
        
    }

    void OnUndoRedo()
    {
        PopulateView(tree);
        AssetDatabase.SaveAssets();
    }

    public NodeView FindNodeView(Node node)
    {
        return GetNodeByGuid(node.guid) as NodeView;
    }

    internal void PopulateView(BehaviorTree tree)
    {

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        clipboard.Clear();

        this.tree = tree;
        if(tree == null)
        {
            return;
        }

        if(tree.rootNode == null)
        {
            tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        //Create node views
        foreach(Node node in tree.nodes)
        {
            CreateNodeView(node);
        }

        //Connect node views (create edges)
        foreach(Node node in tree.nodes)
        {
            NodeView parentView = FindNodeView(node);

            List<Node> children = node.GetChildren();
            foreach(Node child in children)
            {
                NodeView childView = FindNodeView(child);

                Edge edge = parentView.output.ConnectTo(childView.input);
                AddElement(edge);
            }
        }
    }

    GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if(graphViewChange.elementsToRemove != null)
        {
            foreach(GraphElement elem in graphViewChange.elementsToRemove)
            {
                if (elem is NodeView nodeView)
                {
                    tree.DeleteNode(nodeView.node);
                }
                else if (elem is Edge edge)
                {
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;

                    Undo.RecordObject(parentView.node, "Behavior Tree (RemoveChild)");
                    parentView.node.RemoveChild(childView.node);
                    EditorUtility.SetDirty(parentView.node);
                }
                else if (elem is BlackboardField field)
                {
                    blackboard.OnFieldDelete(field);
                }
            }
        }

        if (graphViewChange.edgesToCreate != null)
        {
            foreach (Edge edge in graphViewChange.edgesToCreate)
            {
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;

                Undo.RecordObject(parentView.node, "Behavior Tree (AddChild)");
                parentView.node.AddChild(childView.node);
                parentView.SortChildren();
                EditorUtility.SetDirty(parentView.node);
            }
        }

        if(graphViewChange.movedElements != null)
        {
            foreach(var node in nodes)
            {
                NodeView view = node as NodeView;
                view.SortChildren();
            }
        }


        return graphViewChange;
    }

    NodeView CreateNodeView(Node node)
    {
        NodeView nodeView = new(node, tree.runtime);
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);

        return nodeView;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);

        Type[] baseTypes = { typeof(ActionNode), typeof(CompositeNode), typeof(DecoratorNode) };

        foreach(Type baseType in baseTypes)
        {
            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom(baseType);
            foreach(Type type in types)
            {
                evt.menu.AppendAction($"{type.BaseType.Name}/{type.Name}", (a) => CreateNode(type));
            }
        }
        
    }

    void CreateNode(Type type)
    {
        if(!tree)
        {
            Debug.LogError("Cannot create node without active tree asset.");
            return;
        }
        
        Node node = tree.CreateNode(type);
        CreateNodeView(node);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
                                            // It's not input-input connection            It's not connected to itself
        return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
    }

    public void UpdateNodeStates()
    {
        foreach(var node in nodes)
        {
            NodeView view = node as NodeView;
            view.UpdateState();
        }
    }

}
