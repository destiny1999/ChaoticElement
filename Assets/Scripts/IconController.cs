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
    [SerializeField] BuildingShowInfo buildingShowInfo;

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
        SetBuildingInfoViewInfo(buildingSetting);
        buildingInfoView.SetActive(true);
    }
    public void HideBuildingInfoView()
    {
        buildingInfoView.SetActive(false);
    }
    public void SetBuildingInfoViewInfo(BuildingSetting buildingSetting)
    {
        buildingShowInfo.nameTMP.text = buildingSetting.buildingName;
        buildingShowInfo.damageTMP.text = buildingSetting.damage+ "";
        buildingShowInfo.attackSpeedTMP.text = buildingSetting.attackCD + "";

        string effect = "";
        switch (buildingSetting.SpecialEffect.effect)
        {
            case GameSpecialEffect.SpecialEffect.無特殊效果:
                effect += "無特殊效果";
                break;
            case GameSpecialEffect.SpecialEffect.降低攻擊目標的移動速度:
                effect += "降低攻擊目標的移動緩速";
                break;
            case GameSpecialEffect.SpecialEffect.對攻擊目標造成持續傷害:
                effect += "對攻擊目標造成持續傷害";
                break;
            case GameSpecialEffect.SpecialEffect.攻擊後縮短自身攻擊間隔:
                effect += "每打中一下怪物縮短自身攻擊間隔";
                break;
            case GameSpecialEffect.SpecialEffect.縮短附近防禦塔的攻擊間隔:
                effect += "縮短附近防禦塔的攻擊間隔";
                break;
            case GameSpecialEffect.SpecialEffect.降低目標的防禦:
                effect += "降低目標怪物的防禦";
                break;
        }
        buildingShowInfo.effectTMP.text = effect;
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
[Serializable]
public class BuildingShowInfo
{
    public Image sprite;
    public TextMeshProUGUI nameTMP;
    public TextMeshProUGUI damageTMP;
    public TextMeshProUGUI attackSpeedTMP;
    public TextMeshProUGUI effectTMP;
    public List<GameObject> levelUpList; 
}