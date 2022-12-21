using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicPetController : MonoBehaviour
{
    int nextPosition = 0;
    [SerializeField] List<GameObject> magicPowers;
    [SerializeField] List<Color> magicPowerColors;
    int[] powerNums = new int[6];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DropElement") && !other.GetComponent<DropElementController>().
                                                GetUsedStatus())
        {
            other.GetComponent<DropElementController>().SetUsedStatus();
            int elementIndex = -1;
            GameAttribute.Attribute targetAttribute = 
                other.GetComponent<DropElementController>().GetAttribute().attribute;
            switch (targetAttribute)
            {
                case GameAttribute.Attribute.�L:
                    elementIndex = 0;
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
                SetColor(magicPowerColors[elementIndex]);
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
}
