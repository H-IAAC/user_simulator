using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected;

    public Node node;

    public Port input;
    public Port output;

    public NodeView(Node node) : base("Assets/UI/NodeView.uxml")
    {
        this.node = node;

        this.title = node.name;

        this.viewDataKey = node.guid;

        //Position of the node
        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
        
        SetupClasses();

        Label descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "description";
        descriptionLabel.Bind(new SerializedObject(node));
    }

    void SetupClasses()
    {
        if(node is ActionNode)
        {
            AddToClassList("action");
        }
        else if (node is CompositeNode)
        {
            AddToClassList("composite");
        }
        else if(node is DecoratorNode)
        {
            AddToClassList("decorator");
        }
        else if(node is RootNode)
        {
            AddToClassList("root");
        }
    }

    void CreateInputPorts()
    {
        if(node is ActionNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool)); //Dummy type
        }
        else if (node is CompositeNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool)); //Dummy type
        }
        else if(node is DecoratorNode)
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool)); //Dummy type
        }
        else if(node is RootNode)
        {

        }

        if(input != null)
        {
            input.portName = "";
            input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }
    }

    void CreateOutputPorts()
    {
        if(node is ActionNode)
        {

        }
        else if (node is CompositeNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool)); //Dummy type
        }
        else if(node is DecoratorNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool)); //Dummy type
        }
        else if(node is RootNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool)); //Dummy type
        }

        if(output != null)
        {
            output.portName = "";
            output.style.flexDirection = FlexDirection.ColumnReverse;

            outputContainer.Add(output);
        }
    }

    

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);

        Undo.RecordObject(node, "Behavior Tree (Set Position)");

        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;

        EditorUtility.SetDirty(node);
    }

    public override void OnSelected()
    {
        base.OnSelected();

        if(OnNodeSelected != null)
        {
            OnNodeSelected.Invoke(this);
        }
        
    }

    public void SortChildren()
    {
        CompositeNode composite = node as CompositeNode;
        if(composite)
        {
            composite.children.Sort(SortByHorizontalPostion);
        }
    }

    private int SortByHorizontalPostion(Node left, Node right)
    {
        if(left.position.x < right.position.x)
        {
            return -1;
        }
        
        return 1;
    }

    public void UpdateState()
    {
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");

        if(Application.isPlaying)
        {
            switch (node.state)
            {
                case NodeState.Runnning:
                    if(node.started)
                    {
                        AddToClassList("running");
                    }
                    break;
                case NodeState.Failure:
                    AddToClassList("failure");
                    break;
                case NodeState.Success:
                    AddToClassList("success");
                    break;
            }
        }

        if(node.useMemory)
        {
            AddToClassList("memoried");
            RemoveFromClassList("memoryless");
        }
        else
        {
            RemoveFromClassList("memoried");
            AddToClassList("memoryless");
        }
        
    }
}