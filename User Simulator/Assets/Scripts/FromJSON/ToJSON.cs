using System.IO;
using UnityEditor;
using UnityEngine;

public class ToJSON : MonoBehaviour
{
    #if UNITY_EDITOR
        [MenuItem("Assets/Create/JSON/JSON from Scriptable Object", false, 0)]
        static void SOToTextAsset()
        {
            foreach(UnityEngine.Object obj in Selection.objects)
            {
                ScriptableObject so = (ScriptableObject) obj;
                string json = SOToJSON(so);

                string path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID())); 
                path += "/"+so.name+".json";

                StreamWriter writer = new StreamWriter(path);
                writer.Write(json);
                writer.Close();
            }
            
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/Create/JSON/JSON from Scriptable Object", true)]
        static bool SOToTextAssetValidator()
        {
            foreach(UnityEngine.Object obj in Selection.objects)
            {
                ScriptableObject test = Selection.activeObject as ScriptableObject;
                
                if(test == null)
                {
                    return false;
                }
            }

            return true;
        }
    #endif

    public static string SOToJSON(ScriptableObject so)
    {
        string json = JsonUtility.ToJson(so, true);

        string type_string = ",\n    \"type\": \""+so.GetType().FullName+"\"";

        json = json.Insert(json.Length-2, type_string);

        return json;
    }
}