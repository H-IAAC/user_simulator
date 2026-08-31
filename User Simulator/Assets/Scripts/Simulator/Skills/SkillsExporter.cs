using UnityEngine;
using HIAAC.CstUnity;

/// <summary>
/// Publishes the agent's skill manifest to Redis (skill_manifest memory) so the
/// external agent knows which actions it can request.
/// </summary>
public class SkillsExporter : MonoBehaviour
{
    [SerializeField] private ActionExecutor executor;

    [SerializeField] private UnityMemoryStorage memoryStorage;

    void Start()
    {
        if (executor == null)
        {
            executor = GetComponent<ActionExecutor>();
        }

        if (executor == null || memoryStorage == null)
        {
            Debug.LogWarning("[SkillsExporter] Missing ActionExecutor or UnityMemoryStorage reference");
            return;
        }

        memoryStorage.SetSkillManifest(executor.BuildManifestJson());
    }
}
