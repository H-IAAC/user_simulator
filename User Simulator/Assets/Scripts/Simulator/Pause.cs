using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Perception.Randomization.Scenarios;
using UnityEngine.Perception.GroundTruth;
using System;

public class Pause : MonoBehaviour
{
    float defaultTimeScale;
    bool paused = false;

    public void PauseScene()
    {
        if(paused)
        {
            return;
        }

        defaultTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;

        paused = true;

        ScenarioBase.activeScenario.DelayIteration();

        SetExecutionState("NotStarted");
        
    }

    void SetExecutionState(string value)
    {
        var field = typeof(DatasetCapture).GetField("m_ActiveSimulation", BindingFlags.Static | BindingFlags.NonPublic);
        var state = field.GetValue(null);

        var executionStateField = state.GetType().GetProperty("ExecutionState", BindingFlags.NonPublic | BindingFlags.Instance);
        
        var enumType = state.GetType().GetNestedType("ExecutionStateType", BindingFlags.NonPublic);
        var enumItem = enumType.GetField(value);
        var enumValue = (int)enumItem.GetValue(enumType);
        
        var enumInstance = Enum.ToObject(enumType, enumValue);
        
        executionStateField.SetValue(state, enumInstance);
    }

    public void RunScene()
    {
        if(!paused)
        {
            return;
        }

        Time.timeScale = defaultTimeScale;
        
        paused = false;

        ScenarioBase scenario = ScenarioBase.activeScenario;

        var field = typeof(ScenarioBase).GetField("m_ShouldDelayIteration", 
                                                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        field.SetValue(scenario, false);

        SetExecutionState("Running");
    }

    public void ToggleState()
    {
        if(paused)
        {
            RunScene();
        }
        else
        {
            PauseScene();
        }
    }

    void Update()
    {
    }

}

