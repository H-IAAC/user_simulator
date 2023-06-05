using UnityEngine;

public class DebugPrint : MonoBehaviour
{
    [SerializeField] string text;

    public void Print()
    {
        Debug.Log(text);
    }
}