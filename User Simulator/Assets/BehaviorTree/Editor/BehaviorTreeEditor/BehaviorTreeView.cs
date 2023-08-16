using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace HIAAC.BehaviorTree
{
    public class BehaviorTreeView : GraphView
    {
        public Action<NodeView> OnNodeSelected;

        public new class UxmlFactory : UxmlFactory<BehaviorTreeView, GraphView.UxmlTraits> { }

        public BehaviorTree tree;

        public BlackboardView blackboard;

        List<GraphElement> clipboard;

        List<BehaviorTree> ghostTrees;
        Dictionary<SubtreeNode, BehaviorTree> subtreeNodesVisible;

        Vector2 mousePosition;

        GridBackground grid;

        public BehaviorTreeView()
        {
            grid = new();
            Insert(0, grid);

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new BTViewDoubleClick(this));
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());




            string[] guids = AssetDatabase.FindAssets("t:StyleSheet BehaviorTreeEditor");
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(guids[0]));
            styleSheets.Add(styleSheet);

            Undo.undoRedoPerformed += OnUndoRedo;

            clipboard = new();
            serializeGraphElements += OnCopyCut;
            unserializeAndPaste += OnPaste;

            ghostTrees = new();
            subtreeNodesVisible = new();
        }

        string OnCopyCut(IEnumerable<GraphElement> elements)
        {
            clipboard.Clear();
            clipboard.AddRange(elements);
            return "";
        }

        void OnPaste(string operationName, string data)
        {
            if (operationName == "Duplicate")
            {
                ClearSelection();

                Dictionary<Node, Node> originalToClone = new();

                foreach (GraphElement element in clipboard)
                {
                    if (element is NodeView view)
                    {
                        Node node = view.node;
                        Node clone = tree.DuplicateNode(node);

                        clone.position.x += 10;

                        NodeView cloneView = CreateNodeView(clone);

                        AddToSelection(cloneView);

                        originalToClone.Add(node, clone);
                    }
                }

                //Create connections if duplicating multiple nodes and originals were connected
                foreach (Node node in originalToClone.Keys)
                {
                    if (node.parent == null)
                    {
                        continue;
                    }

                    if (originalToClone.ContainsKey(node.parent))
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

        public void PopulateView(BehaviorTree tree)
        {
            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            clipboard.Clear();
            ghostTrees.Clear();

            this.tree = tree;
            if (tree == null)
            {
                return;
            }

            if (tree.rootNode == null)
            {
                tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
                EditorUtility.SetDirty(tree);
                AssetDatabase.SaveAssets();
            }

            //Create node views
            foreach (Node node in tree.nodes)
            {
                CreateNodeView(node);
            }

            //Connect node views (create edges)
            foreach (Node node in tree.nodes)
            {
                NodeView parentView = FindNodeView(node);

                List<Node> children = node.GetChildren();
                foreach (Node child in children)
                {
                    NodeView childView = FindNodeView(child);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                }
            }
        }

        GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (GraphElement elem in graphViewChange.elementsToRemove)
                {
                    if (elem is NodeView nodeView && nodeView.Ghost == false)
                    {
                        if (nodeView.node is SubtreeNode subtreeNode)
                        {
                            if (subtreeNodesVisible.ContainsKey(subtreeNode))
                            {
                                ToggleSubtreeView(subtreeNode);
                            }
                        }

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

            if (graphViewChange.movedElements != null)
            {
                foreach (var node in nodes)
                {
                    NodeView view = node as NodeView;
                    view.SortChildren();

                    if (view.node is SubtreeNode subtreeNode)
                    {
                        UpdateGhostTree(subtreeNode);
                    }
                }
            }


            return graphViewChange;
        }

        NodeView CreateNodeView(Node node)
        {
            return CreateNodeView(node, tree.runtime);
        }

        NodeView CreateNodeView(Node node, bool runtime, bool ghost = false)
        {
            NodeView nodeView = new(node, runtime, ghost);
            nodeView.OnNodeSelected = OnNodeSelected;
            AddElement(nodeView);

            return nodeView;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //base.BuildContextualMenu(evt);

            Type[] baseTypes = { typeof(ActionNode), typeof(CompositeNode), typeof(DecoratorNode) };

            foreach (Type baseType in baseTypes)
            {
                TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom(baseType);
                foreach (Type type in types)
                {
                    evt.menu.AppendAction($"{baseType.Name}/{type.Name}", (a) => CreateNode(type));
                }
            }

            mousePosition = evt.localMousePosition;

        }

        void CreateNode(Type type)
        {
            if (!tree)
            {
                Debug.LogError("Cannot create node without active tree asset.");
                return;
            }



            Node node = tree.CreateNode(type);
            NodeView view = CreateNodeView(node);

            Vector2 position = grid.ChangeCoordinatesTo(contentViewContainer, mousePosition);
            view.SetPosition(position);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            // It's not input-input connection            It's not connected to itself
            return ports.ToList().Where(endPort => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
        }

        public void UpdateNodeStates()
        {
            foreach (var node in nodes)
            {
                NodeView view = node as NodeView;
                view.UpdateState();
            }
        }

        // Subtree view //---------------------------------------------------------------------------//

        public void ToggleSubtreeView(SubtreeNode subtreeNode)
        {
            if (subtreeNodesVisible.ContainsKey(subtreeNode)) //Already showing this node subtree (even if old changed tree)
            {
                BehaviorTree tree = subtreeNodesVisible[subtreeNode];

                if (ghostTrees.Contains(tree))
                {
                    RemoveGhostTree(tree);
                }

                subtreeNodesVisible.Remove(subtreeNode);
            }
            else
            {
                BehaviorTree staticTree = subtreeNode.Subtree;
                BehaviorTree runtimeTree = subtreeNode.RuntimeTree;

                BehaviorTree[] trees = new[] { runtimeTree, staticTree };

                foreach (BehaviorTree tree in trees)
                {
                    if (tree == null || tree == this.tree)
                    {
                        continue;
                    }

                    if (ghostTrees.Contains(tree)) //Showing tree, but for another node
                    {
                        RemoveGhostTree(tree);

                        SubtreeNode key = subtreeNodesVisible.FirstOrDefault(x => x.Value == tree).Key;
                        subtreeNodesVisible.Remove(key);
                    }

                    //Have tree and is not the editor tree
                    AddGhostTree(tree, subtreeNode);
                    subtreeNodesVisible.Add(subtreeNode, tree);

                    break;
                }
            }
        }



        void RemoveGhostTree(BehaviorTree ghostTree)
        {

            foreach (Node node in ghostTree.nodes)
            {
                NodeView view = FindNodeView(node);

                if (view.output != null)
                {
                    foreach (Edge edge in view.output.connections)
                    {
                        RemoveElement(edge);
                    }

                }

                RemoveElement(view);
            }

            ghostTrees.Remove(ghostTree);
            return;
        }

        void AddGhostTree(BehaviorTree ghostTree, SubtreeNode subtreeNode)
        {
            Vector2 offset = subtreeNode.position + new Vector2(0, 100) - subtreeNode.Subtree.rootNode.position + FindNodeView(subtreeNode).PositionOffset;

            foreach (Node node in ghostTree.nodes)
            {
                NodeView view = CreateNodeView(node, ghostTree.runtime, true);
                view.PositionOffset = offset;
            }

            foreach (Node node in ghostTree.nodes)
            {
                NodeView parentView = FindNodeView(node);

                List<Node> children = node.GetChildren();
                foreach (Node child in children)
                {
                    NodeView childView = FindNodeView(child);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    edge.capabilities = 0;
                    edge.pickingMode = PickingMode.Ignore;
                    AddElement(edge);
                }
            }

            ghostTrees.Add(ghostTree);
        }

        void UpdateGhostTree(SubtreeNode subtreeNode)
        {
            BehaviorTree ghostTree = subtreeNode.Subtree;
            if (ghostTree == null)
            {
                return;
            }

            if (!ghostTrees.Contains(ghostTree))
            {
                return;
            }

            Vector2 offset = subtreeNode.position + new Vector2(0, 100) - subtreeNode.Subtree.rootNode.position;

            foreach (Node node in ghostTree.nodes)
            {
                NodeView view = FindNodeView(node);

                view.PositionOffset = offset;
            }
        }
    }
}