using System.Collections.Generic;
using HIAAC.UserSimulator;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PlotAcceleration : MonoBehaviour
{
    [SerializeField] AccelSensor sensor;
    Plot plot;

    UIDocument document;

    int nPoint = 100;

    List<PlotData> data;

    static Color[] colors = { Color.red, Color.green, Color.blue };

    void Start()
    {
        document = GetComponent<UIDocument>();
        plot = document.rootVisualElement.Query<Plot>();

        data = new();

        for(int i = 0; i<3; i++)
        {
            PlotData curveData = new PlotData
            {
                values = new List<float>(100),
                color = colors[i]
            };

            for(int j = 0; j<100; j++)
            {
                curveData.values.Add(0);
            }

            data.Add(curveData);
        }

        plot.data = data;
    }

    void Update()
    {
        for(int i = 0; i<3; i++)
        {
            data[i].values.RemoveAt(0);
            data[i].values.Add(sensor.Acceleration[i]);
        }

        Debug.Log(sensor.Acceleration);
        
        plot.MarkDirtyRepaint();
    }
}