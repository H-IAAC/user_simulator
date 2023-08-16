using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HIAAC.BehaviorTree
{
    public class BTViewDoubleClick : MouseManipulator
    {
        double time;
        double doubleClickDuration = 0.3;
        BehaviorTreeView view;

        public BTViewDoubleClick(BehaviorTreeView view) : base()
        {
            this.view = view;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        }

        void OnMouseDown(MouseDownEvent evt)
        {
            if (target is not BehaviorTreeView)
            {
                return;
            }

            double duration = EditorApplication.timeSinceStartup - time;
            if (duration < doubleClickDuration)
            {
                NodeView clickedElement = evt.target as NodeView;
                if (clickedElement == null)
                {
                    var ve = evt.target as VisualElement;
                    clickedElement = ve.GetFirstAncestorOfType<NodeView>();
                    if (clickedElement == null)
                    {
                        return;
                    }

                }

                if (clickedElement.node is SubtreeNode subtreeNode)
                {
                    view.ShowSubtree(subtreeNode);
                }


            }

            time = EditorApplication.timeSinceStartup;

        }
    }
}