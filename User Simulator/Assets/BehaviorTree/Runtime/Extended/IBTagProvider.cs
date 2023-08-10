using System.Collections.Generic;

public interface IBTagProvider
{
    public List<BehaviorTag> ProvideTags(List<BTagParameter> agentParameters);
}