using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicPetController : MonoBehaviour
{
    int nextPosition = 0;
    [SerializeField] List<GameObject> magicPowers;
    [SerializeField] List<Color> magicPowerColors;
    int[] powerNums = new int[6];
    public static MagicPetController Instance;
    bool selectingBuff = false;
    [SerializeField] BuffElementSelectorController BuffElementSelector;

    int selectedIndex = -1;
    private void Awake()
    {
        Instance = this;
    }
    // Update is called once per frame
    void Update()
    {
        if (selectingBuff)
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                SelectedFinished();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DropElement") && !other.GetComponent<DropElementController>().
                                                GetUsedStatus())
        {
            bool canClickToChange = false;
            other.GetComponent<DropElementController>().SetUsedStatus();
            int elementIndex = -1;
            GameAttribute.Attribute targetAttribute = 
                other.GetComponent<DropElementController>().GetAttribute().attribute;
            switch (targetAttribute)
            {
                case GameAttribute.Attribute.無:
                    elementIndex = 0;
                    canClickToChange = true;
                    break;
                case GameAttribute.Attribute.水:
                    elementIndex = 1;
                    break;
                case GameAttribute.Attribute.火:
                    elementIndex = 2;
                    break;
                case GameAttribute.Attribute.風:
                    elementIndex = 3;
                    break;
                case GameAttribute.Attribute.光:
                    elementIndex = 4;
                    break;
                case GameAttribute.Attribute.暗:
                    elementIndex = 5;
                    break;
            }
            powerNums[elementIndex]++;
            //print(nextPosition);
            magicPowers[nextPosition].GetComponent<MagicPowerController>().
                SetColor(magicPowerColors[elementIndex], canClickToChange);
            nextPosition++;
            Destroy(other.gameObject);
            GameManager.Instance.SetMagicPetBuff(targetAttribute, powerNums[elementIndex]);
        }
    }
    public int GetAttributePowerNums(GameAttribute.Attribute attribute)
    {
        int nums = 0;
        switch (attribute)
        {
            case GameAttribute.Attribute.水:
                nums = powerNums[1];
                break;
            case GameAttribute.Attribute.火:
                nums = powerNums[2];
                break;
            case GameAttribute.Attribute.風:
                nums = powerNums[3];
                break;
            case GameAttribute.Attribute.光:
                nums = powerNums[4];
                break;
            case GameAttribute.Attribute.暗:
                nums = powerNums[5];
                break;
        }
        return nums;
    }
    public void CallChangeBuffElementsStatus()
    {
        if (!selectingBuff)
        {
            selectingBuff = true;
            BuffElementSelector.ChangeBuffElementsStatus();
        }
    }
    public void SetClickTargetIndex(int index)
    {
        selectedIndex = index;
    }
    public void SelectedFinished()
    {
        BuffElementSelector.ChangeBuffElementsStatus();
        selectingBuff = false;
    }
    
    public void AddMagicPowerBuff(int elementIndex)
    {
        SelectedFinished();

        powerNums[elementIndex]++;
        GameAttribute.Attribute targetAttribute = UseIndexGetAttribute(elementIndex);
        Color color = magicPowerColors[elementIndex];
        color.a = 1;
        magicPowers[selectedIndex].GetComponent<MagicPowerController>().
            SetColor(color, false);
        selectedIndex = -1;
        print(targetAttribute);
        GameManager.Instance.SetMagicPetBuff(targetAttribute, powerNums[elementIndex]);
    }
    GameAttribute.Attribute UseIndexGetAttribute(int index)
    {
        GameAttribute.Attribute target = GameAttribute.Attribute.無;
        switch (index)
        {
            case 1:
                target = GameAttribute.Attribute.水;
                break;
            case 2:
                target = GameAttribute.Attribute.火;
                break;
            case 3:
                target = GameAttribute.Attribute.風;
                break;
            case 4:
                target = GameAttribute.Attribute.光;
                break;
            case 5:
                target = GameAttribute.Attribute.暗;
                break;
        }
        return target;
    }
}
