using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Perception.Randomization.Scenarios;
using UnityEngine.Perception.GroundTruth;
using Unity.MLAgents;
using System;

public class Pause : MonoBehaviour
{
    [SerializeField] BoolVariable gamePausedVariable;

    float defaultTimeScale;
    bool paused = false;

    GameObject academyStepperInner;

    GameObject AcademyStepper
    {
        get
        {
            if(academyStepperInner == null)
            {
                academyStepperInner = GameObject.Find("AcademyFixedUpdateStepper");
            }
            return academyStepperInner;
        }
    }

    void Awake()
    {
        gamePausedVariable.value = false;
    }

    public void PauseScene()
    {
        if(paused)
        {
            return;
        }

        defaultTimeScale = Time.timeScale;
        Time.timeScale = 0.0f;

        paused = true;
        gamePausedVariable.value = true;
        
        if(Utils.PerceptionRunning)
        {
            //Stop scenario iterations
            ScenarioBase.activeScenario.DelayIteration();

            //Avoid skipping timestamp, but captures become with wrong timestamp (DO NOT USE)
            //SetExecutionState("NotStarted");
        }
        if(Utils.MLAgentsRunning)
        {
            AcademyStepper.SetActive(false);
        }
    }

    public void RunScene()
    {
        if(!paused)
        {
            return;
        }

        Time.timeScale = defaultTimeScale;
        
        paused = false;
        gamePausedVariable.value = false;

        if(Utils.PerceptionRunning)
        {
            //Stop scenario iterations
            ScenarioBase scenario = ScenarioBase.activeScenario;
            var field = typeof(ScenarioBase).GetField("m_ShouldDelayIteration", 
                                                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            field.SetValue(scenario, false);

            //Avoid skipping timestamp, but captures become with wrong timestamp (DO NOT USE)
            //SetExecutionState("Running");

            //Avoid wrong dataset size
            var field2 = typeof(ScenarioBase).GetProperty("currentIterationFrame", 
                                                BindingFlags.   Instance | BindingFlags.Public | BindingFlags.NonPublic);
            int value = (int) field2.GetValue(scenario);
            field2.SetValue(scenario, value-1);
        }

        if(Utils.MLAgentsRunning)
        {
            AcademyStepper.SetActive(true);
        }
        
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

}

