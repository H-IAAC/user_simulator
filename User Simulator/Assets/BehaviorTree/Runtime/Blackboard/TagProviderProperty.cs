using UnityEngine;

namespace HIAAC.BehaviorTree
{
    class TagProviderProperty : BlackboardProperty
    {
        [SerializeField]
        [SerializeProperty("Value")]
        UnityEngine.Object tagProviderObj;

        public override object Value
        {
            get
            {
                return tagProviderObj;
            }
            set
            {
                if (value is IBTagProvider)
                {
                    tagProviderObj = value as UnityEngine.Object;
                }
                else
                {
                    tagProviderObj = null;
                }
            }
        }

        public IBTagProvider TagProvider
        {
            get
            {
                return tagProviderObj as IBTagProvider;
            }
        }

        public override string PropertyTypeName
        {
            get
            {
                return "TagProvider";
            }
        }
    }
}