using UnityEngine;


public class EnableAfterFrames : MonoBehaviour
{
    [SerializeField] GameObject objectToEnable;
    [SerializeField] uint framesToWait;

    int waited;

    // Start is called before the first frame update
    void Start()
    {
        waited = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(waited >= framesToWait)
        {
            objectToEnable.SetActive(true);
            enabled = false;
        }

        waited += 1;
    }
}
