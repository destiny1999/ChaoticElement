using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicPetController : MonoBehaviour
{
    int nextPosition = 0;
    [SerializeField] List<GameObject> magicPowers;
    [SerializeField] List<Color> magicPowerColors;
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
            switch (other.GetComponent<DropElementController>().GetAttribute().attribute)
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
            magicPowers[nextPosition].GetComponent<MagicPowerController>().
                SetColor(magicPowerColors[elementIndex]);
            nextPosition++;
            Destroy(other.gameObject);
        }
    }
}
