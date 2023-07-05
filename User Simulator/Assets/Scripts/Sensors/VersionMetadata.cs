using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Perception.GroundTruth;

namespace HIAAC.UserSimulator
{
    /// <summary>
    /// Adds version metadata (scene name, project name, project version) to the generated dataset.
    /// </summary>
    public class VersionMetadata : MonoBehaviour
    {
        /// <summary>
        /// Adds the callback to the simulation ending event.
        /// </summary>
        void Start()
        {
            DatasetCapture.SimulationEnding += Report;
        }

        /// <summary>
        /// Reports the version metadata
        /// </summary>
        void Report()
        {
            Scene scene = SceneManager.GetActiveScene();
            DatasetCapture.ReportMetadata("sceneName", scene.name);

            DatasetCapture.ReportMetadata("projectName", Application.productName);
            DatasetCapture.ReportMetadata("projectVersion", Application.version);
        }
    }
}