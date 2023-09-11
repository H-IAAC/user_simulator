using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class BakeNav : MonoBehaviour
{
    [SerializeField] NavMeshSurface navMeshSurface;
    // Start is called before the first frame update
    void Start()
    {
        navMeshSurface.BuildNavMesh();
        navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
        Debug.Log("Baked");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
