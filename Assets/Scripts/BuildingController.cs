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
    // Start is called before the first frame update
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
    }
    IEnumerator AttackEnemy()
    {
        float attackTime = buildingSetting.attackCD;
        while(targetEnemy != null)
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

                attackTime = buildingSetting.attackCD;
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
