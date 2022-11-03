using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float hp;
    [SerializeField][Tooltip("money")] float mp;
    //[SerializeField] GameObject judgementQuad;
    Dictionary<string, BuildingSetting> buildingSizeDictionary = new Dictionary<string, BuildingSetting>();
    [SerializeField] List<BuildingSetting> buildingSizeSetting = new List<BuildingSetting>();

    [SerializeField] bool prepareCreate = false;
    [SerializeField] bool preparePut = false;
    //[SerializeField] bool waitToPut = false;
    [SerializeField] GameObject targetBuilding = null;

    HashSet<string> buildingCreateStringSet = new HashSet<string>();
    [SerializeField] Transform monsterCreatePosition;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i<buildingSizeSetting.Count; i++)
        {
            buildingSizeDictionary.Add(buildingSizeSetting[i].createButtonString, buildingSizeSetting[i]);
            buildingCreateStringSet.Add(buildingSizeSetting[i].createButtonString);
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
                string inputString = Input.inputString.ToUpper();
                
                if (buildingCreateStringSet.Contains(inputString))
                {
                    if (buildingSizeDictionary[inputString].cost <= mp)
                    {
                        mp -= buildingSizeDictionary[inputString].cost;
                        GameObject newBuilding = Instantiate(buildingSizeDictionary[inputString].building);
                        targetBuilding = newBuilding;
                        preparePut = true;
                        StartCoroutine(MoveAndBuild(newBuilding));
                        
                    }
                }
            }
            
        }

        if (preparePut)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (targetBuilding.GetComponent<BuildingController>().AllPositionOK())
                {
                    preparePut = false;
                    PutBuilding();
                }
            }
        }
    }
    void PutBuilding()
    {
        List<GameObject> judgementQuads = targetBuilding.GetComponent<BuildingController>().GetJudgementQuads();
        

        // set building position to used and disable judgementQuad
        foreach (GameObject quad in judgementQuads)
        {
            quad.GetComponent<JudgementQuadController>().GetCurrentTarget().
                GetComponent<BuildingPositionController>().SetUseSituation(true);
            quad.SetActive(false);
        }

        // put building at positoin
        List<GameObject> buildingPositionQuads = targetBuilding.GetComponent<BuildingController>().
                                                    GetBuildingPositionQuads();

        float maxX = float.MinValue;
        float minX = float.MaxValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;

        foreach (GameObject quad in buildingPositionQuads)
        {
            maxX = quad.transform.position.x > maxX ? quad.transform.position.x : maxX;
            minX = quad.transform.position.x < minX ? quad.transform.position.x : minX;
            maxZ = quad.transform.position.z > maxZ ? quad.transform.position.z : maxZ;
            minZ = quad.transform.position.z < minZ ? quad.transform.position.z : minZ;
        }
        float centerX = (minX + maxX) / 2;
        float centerZ = (minZ + maxZ) / 2;
        Vector3 position = new Vector3(centerX, 0, centerZ);
        targetBuilding.transform.position = position;

        // reset create building status
        prepareCreate = false;
    }
    IEnumerator MoveAndBuild(GameObject building)
    {
        bool put = false;
        RaycastHit hit;
        //waitToPut = true;
        while (!put && preparePut)
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
        preparePut = false;
    }
    public void AddMp(float value)
    {
        mp += value;
    }
    public Transform GetMonsterCreatePosition()
    {
        return monsterCreatePosition;
    }
}


