using UnityEngine;

/// <summary>
/// Prints a debug information to the console.
/// </summary>
public class DebugPrint : MonoBehaviour
{
    [Tooltip("Text to print when Print method is called.")]
    [SerializeField] string text;

    /// <summary>
    /// Prints the debug text
    /// </summary>
    public void Print()
    {
        Debug.Log(text);
    }
}