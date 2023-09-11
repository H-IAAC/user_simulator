
using Mapbox.Unity.MeshGeneration.Data;
using Mapbox.Unity.MeshGeneration.Components;
using Mapbox.Unity.MeshGeneration.Modifiers;
using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.AI.Navigation;

[CreateAssetMenu(menuName = "Mapbox/Modifiers/Add NavMesh Modifier")]
public class AddNavMeshModifier : GameObjectModifier
{
    [SerializeField] int areaType = 0;
    private HashSet<string> _scripts;
    private string _tempId;

    public override void Initialize()
    {
        if (_scripts == null)
        {
            _scripts = new HashSet<string>();
            _tempId = string.Empty;
        }
    }

    public override void Run(VectorEntity ve, UnityTile tile)
    {
        _tempId = ve.GameObject.GetInstanceID() + "NavMeshModifier";
        if (!_scripts.Contains(_tempId))
        {
            NavMeshModifier modifier = ve.GameObject.AddComponent<NavMeshModifier>();
            _scripts.Add(_tempId);

            modifier.overrideArea = true;
            modifier.area = areaType;
        }
    }
}