using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconController : MonoBehaviour
{
    [SerializeField] Color selectedColor;
    [SerializeField] GameObject buildingInfoView;
    [SerializeField] CreateBuildingIconSetting createBuildingIconSetting;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            createBuildingIconSetting.index++;
            if (createBuildingIconSetting.index >= createBuildingIconSetting.elementsName.Count)
            {
                createBuildingIconSetting.index = 0;
            }
            ChangeCreateShowInfo();
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            createBuildingIconSetting.index--;
            if (createBuildingIconSetting.index < 0)
            {
                createBuildingIconSetting.index = createBuildingIconSetting.elementsName.Count-1;
            }
            ChangeCreateShowInfo();
        }
    }
    void ChangeCreateShowInfo()
    {
        createBuildingIconSetting.costTMP.text = createBuildingIconSetting.
                                                    cost[createBuildingIconSetting.index];
        createBuildingIconSetting.nameTMP.text = createBuildingIconSetting.
                                                elementsName[createBuildingIconSetting.index];
        createBuildingIconSetting.targetImage.sprite = createBuildingIconSetting.
                                                chaosElements[createBuildingIconSetting.index];
    }
    public void ChangeIconToBeSelected(GameObject target)
    {
        if(target.GetComponent<Image>().color == Color.white)
        {
            target.GetComponent<Image>().color = selectedColor;
        }
        else
        {
            target.GetComponent<Image>().color = Color.white;
        }
    }
    public void ClickToShowOrHide(GameObject target)
    {
        if (target.activeSelf)
        {
            target.SetActive(false);
        }
        else
        {
            target.SetActive(true);
        }
    }
    public void ShowBuildingInfo(BuildingSetting buildingSetting)
    {
        buildingInfoView.SetActive(true);
    }
    public int GetCreateTargetBuildingCost()
    {
        return int.Parse(createBuildingIconSetting.cost[createBuildingIconSetting.index]);
    }
    public string GetCreateTargetBuildingName()
    {
        return createBuildingIconSetting.elementsName[createBuildingIconSetting.index];
    }
    public int GetCreateTargetLevelIndex()
    {
        return createBuildingIconSetting.index;
    }
}
[Serializable]
public class CreateBuildingIconSetting
{
    public TextMeshProUGUI nameTMP;
    public Image targetImage;
    public TextMeshProUGUI costTMP;
    public int index;
    public List<Sprite> chaosElements;
    public List<string> elementsName;
    public List<string> cost;
}