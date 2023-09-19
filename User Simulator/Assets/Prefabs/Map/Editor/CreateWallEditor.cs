using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CreateWall))]
public class CreateWallEditor: Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CreateWall myScript = (CreateWall)target;
        if(GUILayout.Button("Create"))
        {
            myScript.Create();
        }
    }
}