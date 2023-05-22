using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Perception.GroundTruth;
using UnityEngine.Perception.Randomization.Scenarios;

public class VersionMetadata : MonoBehaviour
{
    void Start()
    {
        DatasetCapture.SimulationEnding += Report;
    }

    void Report()
    {
        Scene scene = SceneManager.GetActiveScene();
        DatasetCapture.ReportMetadata("sceneName", scene.name);

        DatasetCapture.ReportMetadata("projectVersion", Application.version);
    }
}
