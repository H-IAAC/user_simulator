using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFB;

#if UNITY_EDITOR
    using UnityEditor;
#endif


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

        public static ScriptableObject SOFromFilePath(string path, bool save=true)
        {
            StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            reader.Close();

            ScriptableObject so = SOFromJSON(json);
            
            #if UNITY_EDITOR
                string soPath = "Assets/"+Path.GetFileNameWithoutExtension(path)+".asset";
                AssetDatabase.CreateAsset(so, soPath);

                if(save)
                {
                    AssetDatabase.SaveAssets();
                }
            #endif

            return so;
        }

        public static ScriptableObject[] SOFromFilePath(string[] paths, bool save=true)
        {
            ScriptableObject[] objects = new ScriptableObject[paths.Length];
            
            for(int i = 0; i<paths.Length; i++)
            {
                objects[i] = SOFromFilePath(paths[i], false);
            }
            #if UNITY_EDITOR
                AssetDatabase.SaveAssets();
            #endif

            return objects;
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

            //JsonConvert.PopulateObject(json, obj);
            JsonUtility.FromJsonOverwrite(json, obj);

            return obj;
        }

        public delegate void OnLoad(ScriptableObject[] objs);

        public static void askSO(OnLoad callback, bool allowMultiple=false)
        {
            StandaloneFileBrowser.OpenFilePanelAsync("Select JSONs", "", "json", allowMultiple, (paths) => {onFileSelect(callback, paths);});
        }

        static void onFileSelect(OnLoad callback, string[] paths)
        {
            ScriptableObject[] objects = SOFromFilePath(paths);
            callback.Invoke(objects);
        } 

    }
}