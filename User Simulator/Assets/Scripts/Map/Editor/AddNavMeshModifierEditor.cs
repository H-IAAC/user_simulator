using UnityEditor;
using Unity.AI.Navigation.Editor;

[CustomEditor(typeof(AddNavMeshModifier), true)]
class AddNavMeshModifierEditor : Editor
{
    static readonly string[] noDraw = new string[]{
        "areaType"};

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, noDraw);

        SerializedProperty areaType = serializedObject.FindProperty("areaType");

        EditorGUI.indentLevel++;
        NavMeshComponentsGUIUtility.AreaPopup("Area Type", areaType);
        EditorGUI.indentLevel--;
        
        serializedObject.ApplyModifiedProperties();
    }

}