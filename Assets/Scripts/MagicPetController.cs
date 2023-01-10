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
                case GameAttribute.Attribute.�L:
                    elementIndex = 0;
                    canClickToChange = true;
                    break;
                case GameAttribute.Attribute.��:
                    elementIndex = 1;
                    break;
                case GameAttribute.Attribute.��:
                    elementIndex = 2;
                    break;
                case GameAttribute.Attribute.��:
                    elementIndex = 3;
                    break;
                case GameAttribute.Attribute.��:
                    elementIndex = 4;
                    break;
                case GameAttribute.Attribute.�t:
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
            case GameAttribute.Attribute.��:
                nums = powerNums[1];
                break;
            case GameAttribute.Attribute.��:
                nums = powerNums[2];
                break;
            case GameAttribute.Attribute.��:
                nums = powerNums[3];
                break;
            case GameAttribute.Attribute.��:
                nums = powerNums[4];
                break;
            case GameAttribute.Attribute.�t:
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
        GameAttribute.Attribute target = GameAttribute.Attribute.�L;
        switch (index)
        {
            case 1:
                target = GameAttribute.Attribute.��;
                break;
            case 2:
                target = GameAttribute.Attribute.��;
                break;
            case 3:
                target = GameAttribute.Attribute.��;
                break;
            case 4:
                target = GameAttribute.Attribute.��;
                break;
            case 5:
                target = GameAttribute.Attribute.�t;
                break;
        }
        return target;
    }
}
