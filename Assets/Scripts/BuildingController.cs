using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    [SerializeField] List<GameObject> judgementQuads;
    [SerializeField] public BuildingSetting buildingSetting;
    Queue<GameObject> targetQueue = new Queue<GameObject>();
    [SerializeField] GameObject attackDetectRange;
    GameObject targetEnemy = null;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform bulletCreatePosition;
    bool attacking = false;
    bool putted = false;
    [SerializeField] bool beClicked = false;
    List<GameObject> beUsedBuildingPosition = new List<GameObject>();
    // Start is called before the first frame update

    HashSet<int> buffSet = new HashSet<int>();
    HashSet<int> nerfSet = new HashSet<int>();

    [SerializeField] SpecialEffectInfluenceValue specialEffectInfluenceValue;
    Queue<GameObject> willBuffBuildingQueue = new Queue<GameObject>();
    [SerializeField] bool buffBuilding = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(targetQueue.Count > 0 && !attacking)
        {
            targetEnemy = targetQueue.Peek();
            if(targetEnemy != null)
            {
                attacking = true;
                StartCoroutine(AttackEnemy());
            }
            else
            {
                targetQueue.Dequeue();
                transform.rotation = Quaternion.identity;
            }
        }
        else if(willBuffBuildingQueue.Count > 0 && !attacking)
        {
            if(willBuffBuildingQueue.Count > 0 && willBuffBuildingQueue.Peek() != null)
            {
                attacking = true;
                StartCoroutine(BuffBuilding());
            }
        }
    }
    public void SetPutted()
    {
        putted = true;
    }
    public void SetUsePosition(List<GameObject> beUsedPosition)
    {
        beUsedBuildingPosition = beUsedPosition;
    }
    public List<GameObject> GetUsedPosition()
    {
        return beUsedBuildingPosition;
    }
    IEnumerator BuffBuilding()
    {
        float attackTime = buildingSetting.attackCD * 
                            (1 + specialEffectInfluenceValue.attackCDSpeedMagnification/100f);
        
        while(attackTime > 0)
        {
            attackTime -= Time.deltaTime * 1;
            yield return null;
        }
        while (willBuffBuildingQueue.Count > 0)
        {
            if(willBuffBuildingQueue.Peek() == null)
            {
                willBuffBuildingQueue.Dequeue();
                yield return null;
            }
            else
            {
                GameObject targetBuilding = willBuffBuildingQueue.Dequeue();
                willBuffBuildingQueue.Enqueue(targetBuilding);

                if (targetBuilding.GetComponent<BuildingController>().
                    GetStatusSet(true).Contains(buildingSetting.effectIndex))
                {
                    yield return null;
                }
                else
                {
                    targetBuilding.GetComponent<BuildingController>().
                    AddStatus(buildingSetting.effectIndex, buildingSetting.effectValue, true);
                    attacking = false;
                    yield break;
                }
            }
        }
    }
    public HashSet<int> GetStatusSet(bool buff)
    {
        if (buff) return buffSet;
        else return nerfSet;
    }
    IEnumerator AttackEnemy()
    {
        float attackTime = buildingSetting.attackCD *
                            (1 + specialEffectInfluenceValue.attackCDSpeedMagnification / 100f);
        print("magnification = " + (1 + specialEffectInfluenceValue.attackCDSpeedMagnification / 100f));
        print(attackTime);
        while (targetEnemy != null)
        {
            transform.LookAt(targetEnemy.transform);
            attackTime -= Time.deltaTime;
            if(attackTime <= 0)
            {
                GameObject newbullet = Instantiate(bullet);
                newbullet.transform.position = bulletCreatePosition.position;
                Color bulletColor = this.transform.Find("Building").GetComponent<Renderer>().material.color;
                newbullet.GetComponent<BulletController>().
                    SetBulletInfo(buildingSetting.damage,
                                    buildingSetting.effectIndex,
                                    buildingSetting.effectValue,
                                    buildingSetting.bulletSpeed,
                                    bulletColor
                                    );
                newbullet.GetComponent<BulletController>().SetTargetEnemy(targetEnemy);

                attackTime = buildingSetting.attackCD *
                            (1 + specialEffectInfluenceValue.attackCDSpeedMagnification / 100f);
            }
            yield return null;
        }
        attacking = false;
    }
    public bool AllPositionOK()
    {
        bool ok = true;
        foreach(GameObject quad in judgementQuads)
        {
            if (!quad.GetComponent<JudgementQuadController>().CheckPositionStatus())
            {
                ok = false;
                break;
            }
        }
        return ok;
    }
    public List<GameObject> GetJudgementQuads()
    {
        return judgementQuads;
    }
    public List<GameObject> GetBuildingPositionQuads()
    {
        List<GameObject> buildingPositionQuads = new List<GameObject>();
        foreach (GameObject quad in judgementQuads)
        {
            buildingPositionQuads.Add(quad.GetComponent<JudgementQuadController>().
                                        GetCurrentTarget());
        }
        return buildingPositionQuads;
    }
    public void EnableAttackDetectRange()
    {
        attackDetectRange.SetActive(true);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (buffBuilding)
        {
            if (other.transform.CompareTag("building"))
            {
                //print("test");
                willBuffBuildingQueue.Enqueue(other.transform.parent.gameObject);
            }
        }
        else
        {
            if (other.transform.CompareTag("enemy"))
            {
                targetQueue.Enqueue(other.transform.gameObject);
            }
        }
        
        
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("enemy") && targetQueue.Count > 0)
        {
            targetQueue.Dequeue();
            if (targetQueue.Count == 0)
            {
                targetEnemy = null;
                transform.rotation = Quaternion.identity;
            }
        }
    }
    public void SetClickStatus(bool status)
    {
        beClicked = status;
    }
    public bool GetClickStatus()
    {
        return beClicked;
    }
    public void AddStatus(int statusCode, float value, bool buff)
    {
        if (buff) buffSet.Add(statusCode);
        else nerfSet.Add(statusCode);

        ChangeInfluenceValue(statusCode, value, true);
    }
    public void RemoveStatus(int statusCode, float value, bool buff)
    {
        if (buff) buffSet.Remove(statusCode);
        else nerfSet.Remove(statusCode);

        ChangeInfluenceValue(statusCode, value, false);
    }
    void ChangeInfluenceValue(int statusCode, float value, bool add)
    {
        int weight = add ? 1 : -1;
        SpecialEffectSetting specialEffectSetting = GameManager.Instance.
                                                        GetSpecialEffectSetting(statusCode);
        specialEffectSetting.effectValue = value;
        switch (specialEffectSetting.effectInfluenceTarget)
        {
            case SpecialEffectSetting.EffectInfluenceTarget.buildingDamage:
                specialEffectInfluenceValue.damageMagnification += 
                    specialEffectSetting.effectValue * weight;
                break;
            case SpecialEffectSetting.EffectInfluenceTarget.buildingAttackSpeed:
                specialEffectInfluenceValue.attackCDSpeedMagnification +=
                    specialEffectSetting.effectValue * weight * -1;
                break;
        }
    }
}

[Serializable]
public class BuildingSetting
{
    public int buildingCode;
    public float attackCD;
    public float bulletSpeed;
    public float damage;
    public int effectIndex;
    public float effectValue;
    //public int cost;
    public int sale;
    //public string createButtonString;
    public float createTime;
    public int buildingLevel;
    public int areaIndex;
    public List<int> buildingCanCombineCode;

}
[Serializable]
public class BuildingCreateInfo
{
    public GameObject building;
    public int cost;
    public string createString;
}
