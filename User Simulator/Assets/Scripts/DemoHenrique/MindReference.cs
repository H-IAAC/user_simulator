using System;
using UnityEngine;
using HIAAC.CstUnity.Core.Entities;
using System.Collections.Generic;
using HIAAC.CstUnity.MemoryStorage;

[CreateAssetMenu(menuName = "MindReference")]
public class MindReference : ScriptableObject
{
    [System.NonSerialized] Mind mind;

    [SerializeField] bool enableMemoryStorage = true;
    [SerializeField] string redisHost = "127.0.0.1";

    public bool EnableMemoryStorage
    {
        set
        {
            if (mind != null & value != enableMemoryStorage)
            {
                throw new InvalidOperationException("Memory Storage enabled state can only be changed before mind initialization (in inspector, outside play mode).");
            }

            enableMemoryStorage = value;
        }

        get
        {
            return enableMemoryStorage;
        }
    }


    void ensureInitialized()
    {
        if (mind == null)
        {
            mind = new();

            if (enableMemoryStorage)
            {
                MemoryStorageCodelet ms = new(mind, redisConnectionString: redisHost);
                ms.setTimeStep(50);
                mind.insertCodelet(ms);
            }

            mind.start();
        }
    }

    public void addCodelet(Codelet codelet)
    {
        ensureInitialized();

        mind.insertCodelet(codelet);
        codelet.start();
    }

    public Memory getMemory(string name, object info = null)
    {
        ensureInitialized();

        List<Memory> memories = mind.getRawMemory().getAllOfType(name);

        if (memories.Count == 0)
        {
            return mind.createMemoryObject(name, info);
        }
        else
        {
            return memories[0];
        }
    }
}