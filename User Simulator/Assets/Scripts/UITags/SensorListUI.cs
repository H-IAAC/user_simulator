using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HIAAC.UserSimulator;
using TMPro;

public class SensorListUI : UIInfoPanel
{
    [SerializeField] GameObject verticalLayout;

    List<TextMeshProUGUI> texts;

    List<GameObject> textObjects;

    float width;

    public void Start()
    {
        textObjects = new List<GameObject>();
        texts = new List<TextMeshProUGUI>();

        float width = GetComponent<RectTransform>().rect.width;
    }

    public override void Show(GameObject target)
    {
        UniversalSensor[] sensors = target.GetComponentsInChildren<UniversalSensor>();

        int diff = sensors.Length - texts.Count;

        if(diff > 0)
        {
            for(int i = 0; i<diff; i++)
            {
                GameObject go = new GameObject(name, typeof(TextMeshProUGUI));
                TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();

                go.transform.SetParent(verticalLayout.transform, false); 
                go.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

                text.color = Color.black;

                textObjects.Add(go);
                texts.Add(text);
            }
        }
        if(diff < 0)
        {
            for(int i = 0; i<-diff; i++)
            {
                textObjects[textObjects.Count-i-1].SetActive(false);
            }
        }

        for(int i = 0; i<sensors.Length; i++)
        {
            textObjects[i].SetActive(true);

            string name = sensors[i].GroupID+"\\"+sensors[i].ID;
            texts[i].text = name;

            texts[i].SetAllDirty();
        }
        
    }

}
