using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float hp;
    [SerializeField] float mp;
    //[SerializeField] GameObject judgementQuad;
    Dictionary<string, BuildingSetting> buildingSizeDictionary = new Dictionary<string, BuildingSetting>();
    [SerializeField] List<BuildingSetting> buildingSizeSetting = new List<BuildingSetting>();

    [SerializeField] bool prepareCreate = false;
    [SerializeField] bool preparePut = false;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i<buildingSizeSetting.Count; i++)
        {
            buildingSizeDictionary.Add(buildingSizeSetting[i].createButtonString, buildingSizeSetting[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && !prepareCreate)
        {
            prepareCreate = true;
        }
        if (prepareCreate)
        {
            if (!preparePut)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (buildingSizeDictionary["F"].cost <= mp)
                    {
                        mp -= buildingSizeDictionary["F"].cost;
                        GameObject newBuilding = Instantiate(buildingSizeDictionary["F"].building);
                        StartCoroutine(MoveAndBuild(newBuilding));
                        preparePut = true;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.I))
                {
                    if (buildingSizeDictionary["I"].cost <= mp)
                    {
                        mp -= buildingSizeDictionary["I"].cost;
                        GameObject newBuilding = Instantiate(buildingSizeDictionary["I"].building);
                        StartCoroutine(MoveAndBuild(newBuilding));
                        preparePut = true;
                    }
                }
            }
            
        }
        
        
    }
    IEnumerator MoveAndBuild(GameObject building)
    {
        bool put = false;
        RaycastHit hit;
        while (!put)
        {
            Vector3 mousePositoin = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            mousePositoin.z = 1f;
            mousePositoin = Camera.main.ScreenToWorldPoint(mousePositoin);
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 targetPosition = hit.point;
                targetPosition.y = 0.002f;
                building.transform.position = targetPosition;
            }
            yield return null;
        }
        


    }
}

[Serializable]
public class BuildingSetting
{
    public int unitX;
    public int unitY;
    public int cost;
    public string createButtonString;
    public GameObject building;
}
