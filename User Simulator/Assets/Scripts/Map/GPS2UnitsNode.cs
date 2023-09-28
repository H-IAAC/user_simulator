using UnityEngine;
using HIAAC.BehaviorTrees;

public class GPS2UnitsNode : ActionNode
{
    [SerializeField] MapReference mapReference;
    public GPS2UnitsNode()
    {
        CreateProperty(typeof(Vector2BlackboardProperty), "input");
        CreateProperty(typeof(Vector3BlackboardProperty), "output");
    }


    public override void OnStart()
    {
    }

    public override void OnStop()
    {
    }

    public override NodeState OnUpdate()
    {
        if(mapReference == null || mapReference.map == null)
        {
            return NodeState.Failure;
        }

        Vector2 input = GetPropertyValue<Vector2>("input");

        Vector3 output = MapboxUtils.gpsToUnity(mapReference.map, input.x, input.y);

        SetPropertyValue("output", output);

        return NodeState.Success;
    }
}