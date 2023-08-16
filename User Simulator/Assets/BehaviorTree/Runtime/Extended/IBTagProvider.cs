using System.Collections.Generic;

namespace HIAAC.BehaviorTree
{
    public interface IBTagProvider
    {
        public List<BehaviorTag> ProvideTags(List<BTagParameter> agentParameters);
    }
}