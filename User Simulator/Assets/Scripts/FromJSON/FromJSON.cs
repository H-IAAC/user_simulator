using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HIAAC.FromJSON
{
    public class FromJSON
    {
        #if UNITY_EDITOR
            [MenuItem("Assets/Create/JSON/Scriptable Object from JSON", false, 0)]
            static void SOFromTextAsset()
            {
                foreach(UnityEngine.Object obj in Selection.objects)
                {
                    TextAsset text = (TextAsset) obj;
                    string json = text.ToString();

                    ScriptableObject so = SOFromJSON(json);

                    string path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID())); 
                    path += "/"+text.name+".asset";

                    AssetDatabase.CreateAsset(so, path);
                }
                
                AssetDatabase.SaveAssets();
            }
            
            [MenuItem("Assets/Create/JSON/Scriptable Object from JSON", true)]
            static bool SOFromTextAssetValidator()
            {
                foreach(UnityEngine.Object obj in Selection.objects)
                {
                    TextAsset test = Selection.activeObject as TextAsset;
                    if(test == null)
                    {
                        return false;
                    }
                }

                return true;
            }
        #endif

        public static void SOFromFilePath(string path, bool save=true)
        {
            StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            reader.Close();

            ScriptableObject so = SOFromJSON(json);
            
            string soPath = "Assets/"+Path.GetFileNameWithoutExtension(path)+".asset";
            AssetDatabase.CreateAsset(so, soPath);

            if(save)
            {
                AssetDatabase.SaveAssets();
            }
        }

        public static void SOFromFilePath(string[] paths)
        {
            foreach(string path in paths)
            {
                SOFromFilePath(path, false);
            }
            AssetDatabase.SaveAssets();
        }

        public static ScriptableObject SOFromJSON(string json)
        {
            JObject data = JObject.Parse(json);
            
            if(!data.ContainsKey("type"))
            {
                throw new ArgumentException("JSON must contain a 'type' field with Scriptable Object type.");
            }

            string typeName = data["type"].Value<string>();

            ScriptableObject obj = ScriptableObject.CreateInstance(typeName);

            JsonConvert.PopulateObject(json, obj);

            return obj;
        }
    }
}