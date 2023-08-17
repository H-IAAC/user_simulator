using UnityEditor.Experimental.GraphView;
using System;
using UnityEngine.UIElements;

namespace HIAAC.BehaviorTree
{
    /// <summary>
    /// BlackboardField with actions on select and delete.
    /// </summary>
    public class BlackboardField2 : BlackboardField
    {
        public Action OnPropertySelect; //Action to execute on field selected
        
        /// <summary>
        /// Call on select action
        /// </summary>
        public override void OnSelected()
        {
            OnPropertySelect();
        }


    }
}