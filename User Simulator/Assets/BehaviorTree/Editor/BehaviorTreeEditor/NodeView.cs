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
    

    Vector2 positionOffset = Vector2.zero;
    Vector2 positionBase = Vector2.zero;

    bool runtime;

    bool ghost;

    static string UIFilePath = getUIFilePath();

    SerializedObject serializedNode;

    static string getUIFilePath()
    {
        string[] guids = AssetDatabase.FindAssets("t:VisualTreeAsset NodeView");
        return AssetDatabase.GUIDToAssetPath(guids[0]);
    }

    public NodeView(Node node, bool runtime, bool ghost=false) : base(UIFilePath)
    {
        this.node = node;
        serializedNode = new(node);

        this.runtime = runtime;
        this.ghost = ghost;

        if(ghost)
        {
            Selectable = false;
        }

        title = node.name;

        if(title.EndsWith("Node"))
        {
            title = title.Substring(0, title.Length-4);
        }

        viewDataKey = node.guid;
        positionBase = new Vector2(node.position.x, node.position.y);

        //Position of the node
        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
        ConfigurePicking();

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

        if(node is SubtreeNode)
        {
            AddToClassList("subtree-node");
        }
        else
        {
            AddToClassList("base-node");
        }

        if(ghost)
        {
            AddToClassList("ghost");
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

    void ConfigurePicking()
    {
        Port[] ports = { input, output };
        string[] names = { "connector", "cap" };

        foreach(Port port in ports)
        {
            if(port != null)
            {
                foreach(string name in names)
                {
                    VisualElement visualElement = port.Query(name);
                    visualElement.pickingMode = PickingMode.Position;
                }
            }
            
        }
    }

    public Vector2 PositionOffset
    {
        get
        {
            return positionOffset;
        }

        set
        {
            positionOffset = value;

            UpdateViewPosition();
        }
    }

    public override void SetPosition(Rect newPos)
    {
        Undo.RecordObject(node, "Behavior Tree (Set Position)");

        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;

        EditorUtility.SetDirty(node);

        positionBase = newPos.position;
        UpdateViewPosition();
    }

    public void SetPosition(Vector2 positionBase)
    {
        node.position.x = positionBase.x;
        node.position.y = positionBase.y;
        EditorUtility.SetDirty(node);

        SortChildren();

        this.positionBase = positionBase;
        UpdateViewPosition();
    }

    private void UpdateViewPosition()
    {
        //Debug.Log($"{positionBase}|{positionOffset}");
        Vector2 position = positionBase + positionOffset;
        Rect newPos = new(position, Vector2.one);

        base.SetPosition(newPos);
    }

    public override void OnSelected()
    {
        base.OnSelected();

        OnNodeSelected?.Invoke(this);

    }

    public bool Selectable
    {
        set
        {
            if(value)
            {
                capabilities |= Capabilities.Selectable;
            }
            else
            {
                capabilities &= ~Capabilities.Selectable;
            }
        }
        get
        {
            return IsSelectable();
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

        

        if(Application.isPlaying && runtime)
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

            Label utilityValue = (Label) (VisualElement)  this.Query("utility-value");
            utilityValue.text = node.GetUtility().ToString("#0.00");
        }

        if(node.UseMemory)
        {
            AddToClassList("memoried");
            RemoveFromClassList("memoryless");
        }
        else
        {
            RemoveFromClassList("memoried");
            AddToClassList("memoryless");
        }

        RemoveFromClassList("utility");
        AddToClassList("no-utility");
        if(node is CompositeNode composite)
        {
            if(composite.useUtility)
            {
                AddToClassList("utility");
                RemoveFromClassList("no-utility");
            }
        }
    }

    public bool Ghost
    {
        get{ return ghost; }
    }
}