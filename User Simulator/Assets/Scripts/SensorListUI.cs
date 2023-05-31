using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HIAAC.UserSimulator;
using TMPro;

public class SensorListUI : MonoBehaviour
{
    [SerializeField] GameObject baseObject;

    List<TextMeshProUGUI> texts;

    void Start()
    {
        if(baseObject != null)
        {
            float width = GetComponent<RectTransform>().rect.width;

            UniversalSensor[] sensors = baseObject.GetComponentsInChildren<UniversalSensor>();
            
            texts = new List<TextMeshProUGUI>();


            foreach(UniversalSensor sensor in sensors)
            {
                string name = sensor.GroupID+"\\"+sensor.ID;
                GameObject go = new GameObject(name, typeof(TextMeshProUGUI));
                TextMeshProUGUI text = go.GetComponent<TextMeshProUGUI>();

                go.transform.SetParent(this.transform, false); 
                go.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);

                text.text = name;
                text.color = Color.black;

                if(!sensor.enabled)
                {
                    text.fontStyle = FontStyles.Strikethrough;
                }

                text.SetAllDirty();
                texts.Add(text);
            }
        }
    }
}
