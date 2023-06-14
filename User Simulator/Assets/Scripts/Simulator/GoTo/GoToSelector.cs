using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public enum GoToStrategies
{
    Linear,
    NavMesh
}

/// <summary>
/// Selects between multiple avaiable strategies.
/// </summary>
public static class GoToStrategiesExtensions
{
    public static IGoTo Instantiate(this GoToStrategies strategy, GameObject go)
    {
        switch(strategy)
        {
            case GoToStrategies.Linear:
                return go.AddComponent<LinearGoTo>();

            case GoToStrategies.NavMesh:
                return go.AddComponent<NavMeshGoTo>();

            default:
                throw new NotImplementedException(strategy.ToString()+" instantiation not implemented.");
        }
    }

    public static bool ImplementThisStrategy(this GoToStrategies strategy, IGoTo implementation)
    {
        switch(strategy)
        {
            case GoToStrategies.Linear:
                return implementation is LinearGoTo;

            case GoToStrategies.NavMesh:
                return implementation is NavMeshGoTo;

            default:
                return false;
        }
    }
}


public class GoToSelector : MonoBehaviour, IGoTo
{

    IGoTo actualImplementation;

    Dictionary<GoToStrategies, IGoTo> instantiatedStrategies = new Dictionary<GoToStrategies, IGoTo>();

    public Vector3 Destination{get;}

    [SerializeProperty("Strategy")][SerializeField]
    GoToStrategies actualStrategy = GoToStrategies.Linear;



    public GoToStrategies Strategy
    {
        get
        {
            return actualStrategy;
        }

        set
        {
            actualStrategy = value;

            #if UNITY_EDITOR
                if(!EditorApplication.isPlaying)
                {
                    return;
                }
            #endif
            


            if(value.ImplementThisStrategy(actualImplementation))
            {
                return;
            }

            IGoTo instantiated;
            if(!instantiatedStrategies.TryGetValue(value, out instantiated))
            {
                instantiated = value.Instantiate(this.gameObject);

                instantiatedStrategies[value] = instantiated;
            }
            
            instantiated.enabled = true;
            
            if(actualImplementation != null)
            {
                actualImplementation.enabled = false;
            
                if(actualImplementation.Ended == false)
                {
                    instantiated.GoTo(actualImplementation.Destination);
                }
            }
            
            
            actualImplementation = instantiated;
        }
    }
    

    public bool Ended
    {
        get
        {
            return actualImplementation.Ended;
        }
    }

    void Awake()
    {
        actualImplementation = actualStrategy.Instantiate(this.gameObject);
        instantiatedStrategies[actualStrategy] = actualImplementation;
    }

    void IGoTo.goToImplementation(Vector3 destination)
    {

        actualImplementation.GoTo(destination);
    }

}