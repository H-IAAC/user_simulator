using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;

public class BehaviorTreeEditor : EditorWindow
{
    BehaviorTreeView treeView;
    InspectorView inspectorView;
    BlackboardView blackboardView;
    SerializedObject treeObject;

    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Window/AI/Behavior Tree Editor")]
    public static void OpenWindow()
    {
        BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviorTreeEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        m_VisualTreeAsset.CloneTree(root);

        string[] guids = AssetDatabase.FindAssets("t:StyleSheet BehaviorTreeEditor");
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(guids[0]));
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<BehaviorTreeView>();
        inspectorView = root.Q<InspectorView>();
        GenerateBlackboard();

        treeView.OnNodeSelected = (node) => { OnNodeSelectionChanged(node.node); };
        blackboardView.OnPropertySelect = OnNodeSelectionChanged;

        OnSelectionChange();
    }

    void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    void OnPlayModeStateChanged(PlayModeStateChange change)
    {
        switch(change)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;

            case PlayModeStateChange.ExitingEditMode:
                break;

            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;

            case PlayModeStateChange.ExitingPlayMode:
                break;
        }

    }

    void OnSelectionChange()
    {
        if(inspectorView != null)
        {
            inspectorView.Clear();
        }


        BehaviorTree tree = Selection.activeObject as BehaviorTree;

        //If tree not selected, check if GO have tree
        if(!tree && Selection.activeGameObject)
        {
            BehaviorTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviorTreeRunner>();

            if(runner)
            {
                tree = runner.tree;
            }
        }

        if(!tree)
        {
            return;
        }

        tree.Validate();

        if(treeView != null)
        {
            if(Application.isPlaying || AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
            {
                treeView.PopulateView(tree);
                blackboardView.PopulateView(tree);
            }
        }

        treeObject = new SerializedObject(tree);
    }

    void OnNodeSelectionChanged(UnityEngine.Object obj)
    {
        inspectorView.UpdateSelection(obj);
    }


    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if(Selection.activeObject is BehaviorTree)
        {
            OpenWindow();
            return true;
        }
        return false;
    }

    void OnInspectorUpdate()
    {
        treeView.UpdateNodeStates();
    }

    void GenerateBlackboard()
    {
        blackboardView = new BlackboardView(treeView);

        treeView.Add(blackboardView);
        treeView.blackboard = blackboardView;
    }
}
