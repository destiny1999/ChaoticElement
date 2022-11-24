using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpIconController : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] LevelUpInfo levelUpInfo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetLevelUpInfo()
    {
        levelUpInfo.buildingConceptTMP.text = "";
        levelUpInfo.requireBuildingNameTMP.text = "";
    }
}
[Serializable]
public class LevelUpInfo
{
    public GameObject levelUpInfoView;
    public TextMeshProUGUI requireBuildingNameTMP;
    public TextMeshProUGUI buildingConceptTMP;
}
