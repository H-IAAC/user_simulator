using System;
using UnityEngine;
using HIAAC.CstUnity.Core.Entities;
using System.Collections;
using System.Collections.Generic;
using HIAAC.CstUnity.MemoryStorage;
using HIAAC.BehaviorTrees.SmartAreas;
using HIAAC.BehaviorTrees;

public class AffordanceObserver : MonoBehaviour
{
    [SerializeField] MindReference mindReference;

    Memory surroundingActionsMO;


    void Start()
    {
        surroundingActionsMO = mindReference.getMemory("SurroundingActions");
    }

    void Update()
    {
        List<BehaviorTag> tags = AreaManager.instance.GetTags(new(), transform.position);
        List<TagInfo> infos = new();



        foreach (BehaviorTag tag in tags)
        {
            TagInfo info = new()
            {
                name = tag.name,
                description = tag.description,
                originPosition = new float[] { tag.originPosition.x, tag.originPosition.y, tag.originPosition.z },
                originObject = tag.originGameObject.name
            };

            infos.Add(info);
        }

        surroundingActionsMO.setI(infos);

    }


}