using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;

public class BehaviorTreeEditor : EditorWindow
{
    BehaviorTreeView treeView;
    InspectorView inspectorView;

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

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UI/BehaviorTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<BehaviorTreeView>();
        inspectorView = root.Q<InspectorView>();

        treeView.OnNodeSelected = OnNodeSelectionChanged;

        OnSelectionChange();
    }

    void OnSelectionChange()
    {
        BehaviorTree tree = Selection.activeObject as BehaviorTree;

        if(tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
        {
            treeView.PopulateView(tree);
        }
    }

    void OnNodeSelectionChanged(NodeView node)
    {
        inspectorView.UpdateSelection(node);
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
}
