using System;
using UnityEngine;
using HIAAC.CstUnity.Core.Entities;
using System.Collections;
using System.Collections.Generic;
using HIAAC.CstUnity.MemoryStorage;
using HIAAC.BehaviorTrees.SmartAreas;
using HIAAC.BehaviorTrees;

class DemoMind : MonoBehaviour
{
    [SerializeField] string redisHost = "127.0.0.1";

    Mind mind;
    MemoryObject surroundingActionsMO;


    void Start()
    {
        mind = new();

        surroundingActionsMO = mind.createMemoryObject("SurroundingActions");
        surroundingActionsMO.setI(new List<TagInfo>());


        MemoryStorageCodelet ms = new(mind, redisConnectionString: redisHost);
        ms.setTimeStep(50);
        mind.insertCodelet(ms);

        mind.start();
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