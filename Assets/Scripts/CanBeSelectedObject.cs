using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeSelectedObject : MonoBehaviour
{
    public int type;
    public int value;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDown()
    {
        switch (type)
        {
            case 0:// click building
                print("be clicked");
                BuildingSetting buildingSetting = this.GetComponentInParent<BuildingController>().buildingSetting;
                int areaIndex = buildingSetting.areaIndex;
                GameManager.Instance.GetSelfPlayer(areaIndex).GetComponent<PlayerController>().
                    ClickBuilding(buildingSetting, 
                                    this.GetComponentInParent<BuildingController>().gameObject);
                break;
        }
        
    }
}
