using System.Collections.Generic;
using UnityEngine;
using HIAAC.UserSimulator;
using HIAAC.BehaviorTrees;
using TMPro;
using HIAAC.BehaviorTrees.Needs;
using UnityEngine.UI;

public class AgentInfoPanel : UIInfoPanel
{
    [SerializeField] GameObject verticalLayout;

    NeedsContainer needsContainer;

    GameObject needInfoTemplate;
    List<GameObject> needsInfos;
    List<TextMeshProUGUI> needsNames;
    List<TextMeshProUGUI> needsValues;
    List<Image> needsBars;

    public void Awake()
    {
        needInfoTemplate = verticalLayout.transform.GetChild(0).gameObject;
        needInfoTemplate.SetActive(false);

        needsInfos = new()
        {
            needInfoTemplate
        };

        needsNames = new()
        {
            needInfoTemplate.transform.Find("[Text] NeedName").GetComponent<TextMeshProUGUI>()
        };

        needsValues = new()
        {
            needInfoTemplate.transform.Find("[Text] Value").GetComponent<TextMeshProUGUI>()
        };

        needsBars = new()
        {
            needInfoTemplate.transform.Find("[Image] Bar").GetComponent<Image>()
        };
    }

    public override void Show(GameObject target)
    {
        BehaviorTreeRunner runner = target.GetComponent<BehaviorTreeRunner>();
        needsContainer = runner.tree.needsContainer;
        
        //Create missing infos + Disable excess
        int diff = needsContainer.needs.Count - needsInfos.Count;
        if(diff > 0)
        {
            for(int i = 0; i<diff; i++)
            {
                GameObject go = Instantiate(needInfoTemplate);
                go.transform.SetParent(verticalLayout.transform, false); 

                needsInfos.Add(go);
                needsNames.Add(go.transform.transform.Find("[Text] NeedName").GetComponent<TextMeshProUGUI>());
                needsValues.Add(go.transform.transform.Find("[Text] Value").GetComponent<TextMeshProUGUI>());
                needsBars.Add(go.transform.transform.Find("[Image] Bar").GetComponent<Image>());
            }
        }
        if(diff < 0)
        {
            for(int i = 0; i<-diff; i++)
            {
                needsInfos[needsInfos.Count-i-1].SetActive(false);
            }
        }

        for(int i = 0; i<needsContainer.needs.Count; i++)
        {
            needsInfos[i].SetActive(true);
            needsNames[i].text = needsContainer.needs[i].need.name;
            needsValues[i].text = needsContainer.needs[i].value.ToString("0.000");
            needsBars[i].fillAmount = needsContainer.needs[i].value;

            needsNames[i].SetAllDirty();
            needsValues[i].SetAllDirty();
        }
    }

    void Update()
    {
        if(needsContainer != null)
        {
            for(int i = 0; i<needsContainer.needs.Count; i++)
            {
                needsValues[i].text = needsContainer.needs[i].value.ToString("0.000");
                needsBars[i].fillAmount = needsContainer.needs[i].value;

                needsValues[i].SetAllDirty();
            }
        }
    }
}