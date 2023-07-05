using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Synchronizes the button interactable state with a BoolVariable.
/// </summary>
[RequireComponent(typeof(Button))]
public class SyncButtonWithVariable : MonoBehaviour
{
    [Tooltip("Variable to synchronize the button. It is interactable if true.")]
    [SerializeField] BoolVariable variable;

    Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.interactable = variable.value;
    }

    void OnEnable()
    {
        variable.ValueChange.AddListener(this.OnValueChange);
    }

    void OnDisable()
    {
        variable.ValueChange.RemoveListener(this.OnValueChange);
    }

    void OnValidate()
    {
        button = GetComponent<Button>();
        button.interactable = variable.value;
    }

    public void OnValueChange()
    {
        button.interactable = variable.value;
    }
}