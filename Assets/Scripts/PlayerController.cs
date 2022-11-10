using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] int areaIndex;
    [SerializeField] float hp;
    [SerializeField][Tooltip("money")] float mp;
    
    [SerializeField] List<BuildingCreateInfo> buildingCreateInfos;
    Dictionary<string, BuildingCreateInfo> buildingCreateInfoDictionry = new Dictionary<string, BuildingCreateInfo>();
    [SerializeField] bool preparePut = false;
    [SerializeField] GameObject targetBuilding = null;

    [SerializeField] Transform monsterCreatePosition;
    [SerializeField] GameObject buildingManager;
    int challengeScore = 0;

    [SerializeField] PlayerTMPInfo playerTMPInfo;
    [SerializeField] GameObject IconController;
    [SerializeField] Texture2D cursorCombineTexture;

    float willUseMP = 0;
    int willCreateLevel;
    int combineClickCount = 0;
    bool combineStatus = false;

    GameObject levelUpTarget = null;
    GameObject beAbsorbTarget = null;
    BuildingSetting willLevelUpBuildingSetting;
    void Start()
    {

        for(int i = 0; i<buildingCreateInfos.Count; i++)
        {
            buildingCreateInfoDictionry.Add(buildingCreateInfos[i].createString,
                                            buildingCreateInfos[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerTMPInfo.mpTMP.text = mp+"";

        if (!preparePut && !combineStatus)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (IconController.GetComponent<IconController>().GetCreateTargetBuildingCost() <= mp)
                {
                    willUseMP = IconController.GetComponent<IconController>().GetCreateTargetBuildingCost();
                    string targetName = IconController.GetComponent<IconController>().
                                            GetCreateTargetBuildingName();
                    willCreateLevel = IconController.GetComponent<IconController>().GetCreateTargetLevelIndex();
                    CreateBuildingCoin(targetName);
                }
                
            }
        }
        if (preparePut)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (targetBuilding.GetComponent<BuildingController>().AllPositionOK())
                {
                    mp -= willUseMP;
                    willUseMP = 0;
                    preparePut = false;
                    PutBuilding();
                }
            }
        }
        if (!preparePut && Input.GetKeyDown(KeyCode.W))
        {
            combineStatus = true;
            float offset = cursorCombineTexture.width/2;
            Vector2 hotspot = new Vector2(offset, offset);
            Cursor.SetCursor(cursorCombineTexture, hotspot, CursorMode.Auto);
        }

    }
    public void CreateBuilding(string buildingString)
    {
        if (!preparePut)
        {
            if (buildingCreateInfoDictionry[buildingString].cost <= mp)
            {
                mp -= buildingCreateInfoDictionry[buildingString].cost;
                GameObject newBuilding = Instantiate(buildingCreateInfoDictionry[buildingString].building);
                targetBuilding = newBuilding;
                preparePut = true;
                StartCoroutine(MoveAndBuild(newBuilding));
            }
        }
    }
    void CreateBuildingCoin(string targetName)
    {
        ChangeAllBuildingRayLayer(true);

        preparePut = true;
        GameObject newBuilding = Instantiate(buildingCreateInfoDictionry[targetName].building);
        targetBuilding = newBuilding;
        
        StartCoroutine(MoveAndBuild(newBuilding));
    }
    void ChangeAllBuildingRayLayer(bool ignore)
    {
        int layerIndex = ignore ? 2 : 0;
        Transform[] childs = buildingManager.transform.GetComponentsInChildren<Transform>().
            Where(child => child.gameObject.name == "Building").ToArray();
        foreach(Transform transform in childs)
        {
            transform.gameObject.layer = layerIndex;
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
        Destroy(targetBuilding.gameObject);
        //create Random elements building
        CreateRandomBuilding(position);
        ChangeAllBuildingRayLayer(false);
        // reset create building status


    }
    void CreateRandomBuilding(Vector3 createPosition)
    {
        int buildingNums = GameManager.Instance.GetLevelBuildingNums(willCreateLevel);
        int randomIndex = UnityEngine.Random.Range(0, buildingNums);
        
        GameObject building =Instantiate(GameManager.Instance.
            GetBuildingGameObject(willCreateLevel, randomIndex));
        willCreateLevel = -1;
        building.transform.position = createPosition;
        building.transform.SetParent(buildingManager.transform);
        building.GetComponent<BuildingController>().SetPutted();
        building.GetComponent<BuildingController>().buildingSetting.areaIndex = areaIndex;
        // close building's judgement quad.
        Transform[] judgementQuad = building.transform.GetComponentsInChildren<Transform>().
                                        Where(quad => quad.transform.
                                                        CompareTag("buildingJudgePlane")).ToArray();
        foreach(Transform quad in judgementQuad)
        {
            quad.gameObject.SetActive(false);
        }
    }
    void CreateParticularBuilding(GameObject target, Vector3 position)
    {
        GameObject newBuilding = Instantiate(target);
        newBuilding.transform.position = position;
        newBuilding.transform.SetParent(buildingManager.transform);
        newBuilding.GetComponent<BuildingController>().SetPutted();
        newBuilding.GetComponent<BuildingController>().buildingSetting.areaIndex = areaIndex;
        // close building's judgement quad.
        Transform[] judgementQuad = newBuilding.transform.GetComponentsInChildren<Transform>().
                                        Where(quad => quad.transform.
                                                        CompareTag("buildingJudgePlane")).ToArray();
        foreach (Transform quad in judgementQuad)
        {
            quad.gameObject.SetActive(false);
        }
    }
    IEnumerator MoveAndBuild(GameObject building)
    {
        bool put = false;
        RaycastHit hit;
        while (!put && preparePut)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
    public void AddScore(int value)
    {
        challengeScore += value;
    }
    public void SubHP(int value)
    {
        hp -= value;

        //maybe should show some info let player know hp be reduced

        // over
        if (hp <= 0)
        {
            Debug.Log("finished");
        }
    }
    public Transform GetMonsterCreatePosition()
    {
        return monsterCreatePosition;
    }
    public void ClickBuilding(BuildingSetting buildingSetting, GameObject targetBuilding)
    {
        if(!preparePut && !combineStatus) //&& !prepareCreate && !prepareLevelUp)
        {
            GameObject.Find("IconController").GetComponent<IconController>()
                .ShowBuildingInfo(buildingSetting);
        }
        else if (combineStatus)
        {
            targetBuilding.GetComponent<BuildingController>().SetClickStatus(true);
            if(levelUpTarget == null)
            {
                levelUpTarget = targetBuilding;
                willLevelUpBuildingSetting = buildingSetting;
            }
            else
            {
                int beAbsorbLevel = buildingSetting.buildingLevel;
                int beAbsorbCode = buildingSetting.buildingCode;
                if(willLevelUpBuildingSetting.buildingLevel == beAbsorbLevel &&
                    willLevelUpBuildingSetting.buildingCanCombineCode.Contains(beAbsorbCode))
                {
                    beAbsorbTarget = targetBuilding;
                    int code1 = willLevelUpBuildingSetting.buildingCode;
                    int code2 = beAbsorbCode;
                    int level = willLevelUpBuildingSetting.buildingLevel;
                    GameObject newBuilding = GameManager.Instance.
                                                GetNewLevelUpBuilding(level ,code1, code2);
                    CombineTwoBuilding(newBuilding);
                }
                
            }
        }
    }
    void CombineTwoBuilding(GameObject newBuilding)
    {
        levelUpTarget.SetActive(false);
        CreateParticularBuilding(newBuilding, levelUpTarget.transform.position);
        Destroy(levelUpTarget.gameObject);
        Destroy(beAbsorbTarget.gameObject);
        Vector2 hotspot = Vector2.zero;
        Debug.Log("after that should let beabsorb buildign position to can build position");
        Cursor.SetCursor(null, hotspot, CursorMode.Auto);
        combineStatus = false;
    }
}
[Serializable]
public class PlayerTMPInfo
{
    public TextMeshProUGUI mpTMP;
}


