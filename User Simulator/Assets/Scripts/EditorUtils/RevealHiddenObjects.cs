//https://forum.unity.com/threads/gameobjects-deleted-or-invisible-in-hierarchy-but-accessible-by-code.831298/
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
 
/// <summary>
/// Reveals hidden objects in the hierarchy
/// </summary>
public static class RevealHiddenObjects
{
    [MenuItem("Tools/Reveal Hidden GameObjects")]
    private static void RevealHiddenGameObjects()
    {
        //Reveal GO in all loaded scenes
        int countLoaded = SceneManager.sceneCount;
        for (int i = 0; i < countLoaded; i++)
        {
            var scene = SceneManager.GetSceneAt(i);
            foreach(var gameObject in scene.GetRootGameObjects())
            {
                RevealHiddenGameObject(gameObject);
            }

            Debug.Log(scene.name);
        }

        //Reveal also GO in DontDestroyOnLoad scene
        var temp = new GameObject();
        Object.DontDestroyOnLoad( temp );
        Scene dontDestroyOnLoad = temp.scene;
        Object.DestroyImmediate( temp );
        temp = null;
        
        foreach(var gameObject in dontDestroyOnLoad.GetRootGameObjects())
        {
            RevealHiddenGameObject(gameObject);
        }

    }
 
    private static void RevealHiddenGameObject(GameObject gameObject)
    {
        if(gameObject.hideFlags.HasFlag(HideFlags.HideInHierarchy))
        {
            Debug.Log("Revealing hidden GameObject "+gameObject.name, gameObject);
            gameObject.hideFlags &= ~HideFlags.HideInHierarchy;
        }
 
        foreach(Transform child in gameObject.transform)
        {
            RevealHiddenGameObject(child.gameObject);
        }
    }
}