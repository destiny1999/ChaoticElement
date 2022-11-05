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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(targetQueue.Count > 0)
        {
            targetEnemy = targetQueue.Peek();
            transform.LookAt(targetEnemy.transform);
            StartCoroutine(AttackEnemy());
        }
    }
    IEnumerator AttackEnemy()
    {
        float attackTime = buildingSetting.attackCD;
        while(targetEnemy != null)
        {
            attackTime -= Time.deltaTime;
            print("attack time " + attackTime);
            if(attackTime <= 0)
            {
                GameObject newbullet = Instantiate(bullet);
                newbullet.transform.position = bulletCreatePosition.position;
                newbullet.GetComponent<BulletController>().
                    SetBulletInfo(buildingSetting.damage,
                                    buildingSetting.effectIndex,
                                    buildingSetting.effectValue,
                                    buildingSetting.bulletSpeed);
                newbullet.GetComponent<BulletController>().SetTargetEnemy(targetEnemy);

                attackTime = buildingSetting.attackCD;
            }
            yield return null;
        }
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
    public void OnCollisionEnter(Collision collision)
    {
        
    }
    public void OnTriggerEnter(Collider other)
    {
        
        if (other.transform.CompareTag("enemy"))
        {
            targetQueue.Enqueue(other.transform.gameObject);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("enemy"))
        {
            targetQueue.Dequeue();
            if (targetQueue.Count == 0)
            {
                targetEnemy = null;
                transform.rotation = Quaternion.identity;
            }
        }
    }
}

[Serializable]
public class BuildingSetting
{
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
}
[Serializable]
public class BuildingCreateInfo
{
    public GameObject building;
    public int cost;
    public string createString;
}
