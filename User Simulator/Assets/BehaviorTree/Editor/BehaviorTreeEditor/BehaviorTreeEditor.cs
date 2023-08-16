using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Callbacks;

namespace HIAAC.BehaviorTree
{
    /// <summary>
    /// Main BT Editor Windows class
    /// </summary>
    public class BehaviorTreeEditor : EditorWindow
    {
        BehaviorTreeView treeView; //Main tree view
        InspectorView inspectorView; //Lateral inspector view
        BlackboardView blackboardView; //Blackboard view over the treeView
        InspectorView agentParameters; //Lateral BTag parameters view
        
        SerializedObject treeObject; //Active tree asset

        private VisualTreeAsset m_VisualTreeAsset; //UXML asset


        /// <summary>
        /// Creates option to open the editor in the menu.
        /// </summary>
        [MenuItem("Window/AI/Behavior Tree Editor")]
        public static void OpenWindow()
        {
            BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
            wnd.titleContent = new GUIContent("BehaviorTreeEditor");
        }

        /// <summary>
        /// Creates the editor
        /// </summary>
        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            
            //Get the UXML object
            if(m_VisualTreeAsset == null)
            {
                string[] guidsVisualTree = AssetDatabase.FindAssets("t:VisualTreeAsset BehaviorTreeEditor");
                m_VisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath(guidsVisualTree[0]));
            }

            // Instantiate UXML
            m_VisualTreeAsset.CloneTree(root);

            //Gets the USS
            string[] guids = AssetDatabase.FindAssets("t:StyleSheet BehaviorTreeEditor");
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath(guids[0]));
            root.styleSheets.Add(styleSheet);

            //Get the reference to each editor piece
            treeView = root.Q<BehaviorTreeView>();
            inspectorView = root.Q<InspectorView>("inspector");
            agentParameters = root.Q<InspectorView>("agent-parameters");
            GenerateBlackboard();

            //Define the delegate methods
            treeView.OnNodeSelected = (node) => { OnNodeSelectionChanged(node.node); };
            blackboardView.OnPropertySelect = OnNodeSelectionChanged;

            //Update the view
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
            switch (change)
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
            if (inspectorView != null)
            {
                inspectorView.Clear();
            }


            BehaviorTree tree = Selection.activeObject as BehaviorTree;

            //If tree not selected, check if GO have tree
            if (!tree && Selection.activeGameObject)
            {
                BehaviorTreeRunner runner = Selection.activeGameObject.GetComponent<BehaviorTreeRunner>();

                if (runner)
                {
                    tree = runner.tree;
                }
            }

            if (!tree)
            {
                return;
            }

            tree.Validate();
            treeObject = new SerializedObject(tree);

            if (treeView != null)
            {
                if (Application.isPlaying || AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
                {
                    treeView.PopulateView(tree);
                    blackboardView.PopulateView(tree);
                    agentParameters.UpdateSelection(treeObject.FindProperty("bTagParameters"));
                }
            }
        }

        void OnNodeSelectionChanged(UnityEngine.Object obj)
        {
            inspectorView.UpdateSelection(obj);
        }


        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is BehaviorTree)
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
}