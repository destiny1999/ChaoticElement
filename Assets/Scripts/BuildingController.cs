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

    HashSet<GameSpecialEffect.SpecialEffect> buffSet = new HashSet<GameSpecialEffect.SpecialEffect>();
    HashSet<GameSpecialEffect.SpecialEffect> nerfSet = new HashSet<GameSpecialEffect.SpecialEffect>();


    //[SerializeField] BuildingSpecialEffectInfluenceValue specialEffectInfluenceValue;
    Queue<GameObject> willBuffBuildingQueue = new Queue<GameObject>();
    [SerializeField] bool buffBuilding = false;
    [SerializeField] bool allAttack = false;
    [SerializeField] bool notAttack = false;
    GameObject centerAllAttackTarget;

    private void Awake()
    {
        //print("initial orignal");
        buildingSetting.originalDamage = buildingSetting.damage;
    }
    private void Start()
    {
        if (allAttack)
        {
            centerAllAttackTarget = GameManager.Instance.allAttackCenterTarget;
            bulletCreatePosition.position = GameManager.Instance.allAttackBulletCreatePosition.position;
            targetEnemy = centerAllAttackTarget;
            StartCoroutine(AttackEnemy());
        }
        else if(notAttack && buildingSetting.Attribute.attribute == GameAttribute.Attribute.火)
        {
            GameManager.Instance.CallChangeRingsCount(1, true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(targetQueue.Count > 0 && !attacking && !allAttack)
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
            }
        }
        else if(willBuffBuildingQueue.Count > 0 && !attacking)
        {
            if(willBuffBuildingQueue.Count > 0 && willBuffBuildingQueue.Peek() != null)
            {
                print("into buff");
                attacking = true;
                StartCoroutine(BuffBuilding());
            }
        }
    }
    IEnumerator ResetRotation()
    {
        while (transform.eulerAngles != Vector3.zero && targetEnemy == null)
        {
            transform.rotation = Quaternion.identity;
            yield return null;
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
        //print("use buff");
        float attackTime = buildingSetting.attackCD * 
                            (1 + buildingSetting.SpecialEffectInfluenceValue.
                                attackCDSpeed/100f);
        
        while(attackTime > 0)
        {
            attackTime -= Time.deltaTime * 1;
            yield return null;
        }
        while (willBuffBuildingQueue.Count > 0)
        {
            print("buff count > 0");
            if(willBuffBuildingQueue.Peek() == null)
            {
                willBuffBuildingQueue.Dequeue();
                yield return null;
            }
            else
            {
                GameObject targetBuilding = willBuffBuildingQueue.Dequeue();
                willBuffBuildingQueue.Enqueue(targetBuilding);
                for(int i = 0; i< buildingSetting.SpecialEffects.Count; i++)
                {
                    if (targetBuilding.GetComponent<BuildingController>().
                    GetStatusSet(true).Contains(buildingSetting.SpecialEffects[i].effect))
                    {
                        //yield return null;
                    }
                    else
                    {
                        targetBuilding.GetComponent<BuildingController>().
                            AddStatus(buildingSetting.SpecialEffects[i], true);
                        attacking = false;
                    }
                    
                }
                break;
            }
        }
        attacking = false;
    }
    public HashSet<GameSpecialEffect.SpecialEffect> GetStatusSet(bool buff)
    {
        if (buff) return buffSet;
        else return nerfSet;
    }
    IEnumerator AttackEnemy()
    {
        if (notAttack) yield break;
        float attackTime = buildingSetting.attackCD *
                            (1 + buildingSetting.SpecialEffectInfluenceValue.
                                attackCDSpeed / 100f);
        int reduceAttackTimeWeight = 1;
        buildingSetting.damage = buildingSetting.originalDamage;
        int attackTimes = 0;
        float targetAttackTimes = -1;
        bool specialAttack = false;
        while (targetEnemy != null)
        {
            attackTime -= Time.deltaTime;
            if(attackTime <= 0)
            {
                if(attackTimes == targetAttackTimes)
                {
                    attackTimes = 0;
                    specialAttack = true;
                }
                GameObject newbullet = Instantiate(bullet);
                newbullet.transform.position = bulletCreatePosition.position;
                Color bulletColor = buildingSetting.bulletColor;
                
                newbullet.GetComponent<BulletController>().
                    SetBulletInfo(buildingSetting.damage * 
                                    (1 + buildingSetting.SpecialEffectInfluenceValue.damage / 100f),
                                    buildingSetting.Attribute,
                                    buildingSetting.SpecialEffects,
                                    buildingSetting.bulletSpeed,
                                    bulletColor
                                    );
                if (buffSet.Contains(GameSpecialEffect.SpecialEffect.攻擊時有機會雙倍傷害))
                {
                    print("contain");
                    GameSpecialEffect newEffect = new GameSpecialEffect();
                    newEffect.effect = GameSpecialEffect.SpecialEffect.攻擊時有機會雙倍傷害;
                    newbullet.GetComponent<BulletController>().bulletSetting.SpecialEffects.
                        Add(newEffect);
                }
                if (specialAttack)
                {
                    specialAttack = false;
                    newbullet.GetComponent<BulletController>().bulletSetting.damage =
                        newbullet.GetComponent<BulletController>().bulletSetting.damage * 10;
                }

                newbullet.GetComponent<BulletController>().SetTargetEnemy(targetEnemy);

                attackTime = buildingSetting.attackCD *
                            (1 + buildingSetting.SpecialEffectInfluenceValue.
                                attackCDSpeed / 100f);
                if(buildingSetting.GetTargetSpecialEffect
                    (GameSpecialEffect.SpecialEffect.攻擊後縮短自身攻擊間隔) != null)
                {
                    attackTime -= buildingSetting.GetTargetSpecialEffect
                        (GameSpecialEffect.SpecialEffect.攻擊後縮短自身攻擊間隔).effectValue *
                        reduceAttackTimeWeight;
                    reduceAttackTimeWeight++;
                }
                if(buildingSetting.GetTargetSpecialEffect
                    (GameSpecialEffect.SpecialEffect.攻擊後提升自身攻擊傷害) != null)
                {
                    buildingSetting.damage += buildingSetting.GetTargetSpecialEffect
                            (GameSpecialEffect.SpecialEffect.攻擊後提升自身攻擊傷害).effectValue;
                }
                if (buildingSetting.GetTargetSpecialEffect
                    (GameSpecialEffect.SpecialEffect.攻擊一定次數後爆擊傷害) != null)
                {
                    targetAttackTimes = buildingSetting.GetTargetSpecialEffect
                    (GameSpecialEffect.SpecialEffect.攻擊一定次數後爆擊傷害).effectValue;
                    attackTimes++;
                }
                if (attackTime < 0) attackTime = 0;
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
        if (allAttack) return;
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
        if (allAttack) return;
        if (other.transform.CompareTag("enemy") && targetQueue.Count > 0)
        {
            StartCoroutine(CheckTargetQueue(targetQueue));
        }
        else if(other.transform.CompareTag("building") && willBuffBuildingQueue.Count > 0)
        {
            StartCoroutine(CheckTargetQueue(willBuffBuildingQueue));
        }
    }
    IEnumerator CheckTargetQueue(Queue<GameObject> targetQueue)
    {
        while(targetQueue.Count > 0 && targetQueue.Peek() == null)
        {
            targetQueue.Dequeue();
            yield return null;
        }
        if (targetQueue.Count > 0)
        {
            targetQueue.Dequeue();
        }
        if (targetQueue.Count == 0)
        {
            targetEnemy = null;
        }
        else
        {
            targetEnemy = targetQueue.Peek();
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
    /// <summary>
    /// the buff from building
    /// </summary>
    /// <param name="gameSpecialEffect"></param>
    /// <param name="buff"></param>
    public void AddStatus(GameSpecialEffect gameSpecialEffect, bool buff)
    {
        if (buff) buffSet.Add(gameSpecialEffect.effect);
        else nerfSet.Add(gameSpecialEffect.effect);
        buildingSetting.SpecialEffectInfluenceValue.
            ChangeInfluenceValue(gameSpecialEffect ,true);
        if(gameSpecialEffect.effectKeepTime > 0)
        {
            StartCoroutine(ReduceStatusTime(gameSpecialEffect));
        }
        //ChangeInfluenceValue(gameSpecialEffect, true);
    }
    IEnumerator ReduceStatusTime(GameSpecialEffect gameSpecialEffect)
    {
        float time = gameSpecialEffect.effectKeepTime;
        while(time > 0)
        {
            //print("time = " + time);
            time -= Time.deltaTime * 1;
            yield return null;
        }
        RemoveStatus(gameSpecialEffect, true);
    }
    /// <summary>
    /// the magic pet collect power to buff specific buliding
    /// </summary>
    public void AddMagicPetBuff(int nums)
    {
        //print(buildingSetting.originalDamage);
        

        buildingSetting.damage = buildingSetting.originalDamage + 
                                    buildingSetting.originalDamage / 2 * nums;
    }
    public void RemoveStatus(GameSpecialEffect gameSpecialEffect, bool buff)
    {
        //print("remove");
        if (buff)
            CheckTargetToRemove(buffSet, gameSpecialEffect);
        //buffSet.Remove(gameSpecialEffect.effect);
        else
            CheckTargetToRemove(nerfSet, gameSpecialEffect);
            //nerfSet.Remove(gameSpecialEffect.effect);
        buildingSetting.SpecialEffectInfluenceValue.ChangeInfluenceValue(gameSpecialEffect, false);
    }
    void CheckTargetToRemove(HashSet<GameSpecialEffect.SpecialEffect> set ,GameSpecialEffect specialEffect)
    {
        foreach(GameSpecialEffect.SpecialEffect gse in set)
        {
            if(gse == specialEffect.effect)
            {
                set.Remove(gse);
                break;
            }
        }
    }
}

[Serializable]
public class BuildingSetting : GameItemInfo
{
    public string buildingName;
    public int buildingCode;
    public float attackCD;
    public float bulletSpeed;
    [HideInInspector]public float originalDamage;
    public float damage;
    public int sale;
    public float createTime;
    public int buildingLevel;
    public int areaIndex;
    public List<int> buildingCanCombineCode;
    public Color bulletColor;

}
[Serializable]
public class BuildingCreateInfo
{
    public GameObject building;
    public int cost;
    public string createString;
}
