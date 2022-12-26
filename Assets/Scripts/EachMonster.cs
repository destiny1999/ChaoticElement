using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HPMonitorController))]
public class EachMonster : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] MonsterSettingNew monsterSetting;
    Monster monster;
    Transform nextTargetTransform;
    public bool dead = false;
    public bool notMove = false;
    bool burning = false;
    float moveTime = 0;
    [SerializeField]List<Status> statusList = new List<Status>();
    private void Awake()
    {

    }
    void Start()
    {
        monster = new Monster(monsterSetting);
        if (notMove) return;
        nextTargetTransform = GameObject.Find("FirstMoveCheckPoint").gameObject.transform;
        
    }
    private void Update()
    {
        //moveTime += Time.deltaTime * 1;

        monster.SpecialEffectInfluenceValue.speed = statusList[0].value;
        monster.SpecialEffectInfluenceValue.hp = statusList[1].value;
        monster.SpecialEffectInfluenceValue.defense = statusList[2].value;



        if (notMove) return;
        float moveSpeed = monster.speed * (1 - (monster.SpecialEffectInfluenceValue.speed / 100f));
        transform.position = Vector3.MoveTowards(transform.position, nextTargetTransform.position,
                            moveSpeed * Time.deltaTime);
        
        transform.LookAt(nextTargetTransform);
        if (Vector3.Distance(transform.position, nextTargetTransform.position) <= 0.1f)
        {
            if (!nextTargetTransform.GetComponent<PathNodeController>().CheckLastNode())
            {
                nextTargetTransform = nextTargetTransform.GetComponent<PathNodeController>().
                                        GetNext();
            }
            else
            {
                GameManager.Instance.SendKillStatus(monster);
                //print("at the end " + moveTime);
                Destroy(gameObject);
            }
        }
        // 
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!dead)
        {
            if (other.transform.CompareTag("bullet") && 
                    !other.GetComponent<BulletController>().bulletSetting.used)
            {
                if (!other.GetComponent<BulletController>().rangeAttack)
                {
                    other.GetComponent<BulletController>().bulletSetting.used = true;
                }
                float damage = other.transform.GetComponent<BulletController>().bulletSetting.damage;
                GameAttribute enemyAttribute = other.transform.GetComponent<BulletController>().
                                                            bulletSetting.Attribute;

                if (other.transform.GetComponent<BulletController>().bulletSetting.
                        SpecialEffect.effect != GameSpecialEffect.SpecialEffect.無特殊效果)
                {
                    /*monster.DealWithSpecialEffectValue(other.transform.
                        GetComponent<BulletController>().bulletSetting.SpecialEffect);*/
                    DealWithStatus(other.transform.
                                    GetComponent<BulletController>().bulletSetting.SpecialEffect);
                    if(monster.SpecialEffectInfluenceValue.hp != 0 && !burning)
                    {
                        StartCoroutine(DealWithBurn());
                    }
                }
                damage = monster.CaculateDamage(damage, enemyAttribute);
                //print(damage);
                if (!other.GetComponent<BulletController>().rangeAttack)
                {
                    Destroy(other.gameObject);
                }
                ReduceHP(damage);
            }
        }
        
    }
    void DealWithStatus(GameSpecialEffect bulletSpecialEffect)
    {
        Status newStatus = new Status();
        newStatus.value = bulletSpecialEffect.effectValue;
        newStatus.time = bulletSpecialEffect.effectKeepTime;

        switch (bulletSpecialEffect.effect)
        {
            case GameSpecialEffect.SpecialEffect.攻擊時有機會凍住敵人:
                newStatus.target = Status.InfluenceStatus.speed;
                if (bulletSpecialEffect.effectLevel < 5)
                {
                    if (UnityEngine.Random.Range(0f, 100f) <= bulletSpecialEffect.effectValue)
                    {
                        statusList[0].value = 100f;
                        if (statusList[0].time == 0)
                        {
                            StartCoroutine(ReduceStatusRemainTime(0, newStatus));
                        }
                        else
                        {
                            statusList[0].time = newStatus.time;
                        }
                    }
                }
                else
                {
                    UseLevel5Ice(newStatus);
                    
                }
                break;

            case GameSpecialEffect.SpecialEffect.降低攻擊目標的移動速度:
                newStatus.target = Status.InfluenceStatus.speed;

                if (statusList[0].value <= bulletSpecialEffect.effectValue)
                {
                    statusList[0].value = bulletSpecialEffect.effectValue;
                    if (statusList[0].time == 0)
                    {
                        StartCoroutine(ReduceStatusRemainTime(0, newStatus));
                    }
                    else
                    {
                        statusList[0].time = newStatus.time;
                    }
                }
                /*
                monster.SpecialEffectInfluenceValue.speed = monster.SpecialEffectInfluenceValue.speed >
                                                        bulletSpecialEffect.effectValue ?
                                                        monster.SpecialEffectInfluenceValue.speed :
                                                        bulletSpecialEffect.effectValue;*/
                break;
            case GameSpecialEffect.SpecialEffect.對攻擊目標造成持續傷害:
                newStatus.target = Status.InfluenceStatus.hp;
                if (statusList[1].value <= bulletSpecialEffect.effectValue)
                {
                    statusList[1].value = bulletSpecialEffect.effectValue;
                    
                    if (statusList[1].time == 0 )
                    {
                        StartCoroutine(ReduceStatusRemainTime(1, newStatus));
                    }
                    else
                    {
                        statusList[1].time = newStatus.time;
                    }
                    
                }
                /*
                monster.SpecialEffectInfluenceValue.hp = monster.SpecialEffectInfluenceValue.hp >
                                                    bulletSpecialEffect.effectValue ?
                                                    monster.SpecialEffectInfluenceValue.hp :
                                                    bulletSpecialEffect.effectValue;*/
                //Debug.Log("hp influence");
                break;
            case GameSpecialEffect.SpecialEffect.降低目標的防禦:
                //newStatus.target = Status.InfluenceStatus.defense;
                if(bulletSpecialEffect.effectLevel < 4)
                {
                    statusList[2].value += bulletSpecialEffect.effectValue;
                }
                else if(bulletSpecialEffect.effectLevel == 4)
                {
                    if (statusList[2].value == 0)
                    {
                        statusList[2].value = monster.defense / 2;
                    }
                    else
                    {
                        statusList[2].value += (monster.defense - statusList[2].value) / 2f;
                    }

                    if (statusList[2].value >= 
                            monster.defense * (100-bulletSpecialEffect.effectValue) / 100f)
                    {
                        statusList[2].value = monster.defense;
                    }
                }
                break;
                /*
                if (statusList[2].value <= bulletSpecialEffect.effectValue)
                {
                    statusList[2].value = bulletSpecialEffect.effectValue;
                    if (statusList[2].time == 0)
                    {
                        StartCoroutine(ReduceStatusRemainTime(2, newStatus));
                    }
                    else
                    {
                        statusList[2].time = newStatus.time;
                    }
                }*/

                /*
                monster.SpecialEffectInfluenceValue.defense = monster.SpecialEffectInfluenceValue.defense >
                                                    bulletSpecialEffect.effectValue ?
                                                    monster.SpecialEffectInfluenceValue.defense :
                                                    bulletSpecialEffect.effectValue;
                break;*/
                /*
            case GameSpecialEffect.SpecialEffect.攻擊時有機會凍住敵人:
                float rate = bulletSpecialEffect.effectValue;
                if (UnityEngine.Random.Range(0f, 100f) <= rate)
                {
                    monster.SpecialEffectInfluenceValue.speed = 100f;
                }
                break;*/
        }
        //yield return null;
    }
    void UseLevel5Ice(Status status)
    {
        int times = 0;
        for(int i = 0; i<5; i++)
        {
            if (UnityEngine.Random.Range(0f, 100f) <= 50f)
            {
                times++;
            }
        }
        if (statusList[0].value <= times * 20f)
        {
            statusList[0].value = times * 20f;
            if (statusList[0].time == 0)
            {
                StartCoroutine(ReduceStatusRemainTime(0, status));
            }
            else
            {
                statusList[0].time = status.time;
            }

        }
    }

    /// <summary>
    /// if this status remain time less than 0, will remove status.
    /// 0 speed, 1 hp, 2 defens
    /// </summary>
    /// <param name="targetIndex"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    IEnumerator ReduceStatusRemainTime(int targetIndex, Status status)
    {
        statusList[targetIndex].time = status.time;
        while (statusList[targetIndex].time > 0)
        {
            statusList[targetIndex].time -= Time.deltaTime * 1;
            yield return null;
        }
        statusList[targetIndex].time = 0;
        statusList[targetIndex].value = 0;
    }
    IEnumerator DealWithBurn()
    {
        //print("into burn");
        burning = true;
        float time = 1;
        while(monster.SpecialEffectInfluenceValue.hp > 0)
        {
            time -= Time.deltaTime * 1;
            if(time <= 0)
            {
                ReduceHP(monster.SpecialEffectInfluenceValue.hp);
                time = 1;
                //print("reduce because of burn");
            }
            yield return null;
        }
    }
    void ReduceHP(float damage)
    {
        monster.HP -= damage;
        this.GetComponent<HPMonitorController>().ChangeHpShowValue(monster.HP / monsterSetting.hp);
        if (monster.HP <= 0)
        {
            dead = true;
            GameManager.Instance.SendKillStatus(monster);
            bool drop = UnityEngine.Random.Range(0f, 100f) <= monster.elementDropRate ? true : false;
            if (drop) GameManager.Instance.CreateDropElement(monster.Attribute.attribute,
                                                                transform.localPosition);
            GameManager.Instance.CreateEarnMoney(monster.killBonuse, transform.localPosition);
            Destroy(gameObject);
        }
    }
    public MonsterSettingNew GetMonsterSetting()
    {
        return monsterSetting;
    }
}

public abstract class MonsterBase : GameItemInfo
{
    string _name;
    float _hp;
    float _speed;
    int _damage;
    float _killBonuse;
    float _defense;
    public abstract float CaculateDamage(float damage, GameAttribute enemyAttribute);
    public float elementDropRate { get; set; }
    public float HP
    {
        get => _hp;
        set => _hp = value;
    }
    public string monsterName
    {
        get => _name;
        set => _name = value;
    }
    public float speed
    {
        get => _speed;
        set => _speed = value;
    }
    public int damage
    {
        get => _damage;
        set => _damage = value;
    }
    public float killBonuse
    {
        get => _killBonuse;
        set => _killBonuse = value;
    }
    public float defense
    {
        get => _defense;
        set => _defense = value;
    }
    /*
    public void DealWithSpecialEffectValue(GameSpecialEffect bulletSpecialEffect)
    {
        switch (bulletSpecialEffect.effect)
        {
            case GameSpecialEffect.SpecialEffect.降低攻擊目標的移動速度:
                SpecialEffectInfluenceValue.speed = SpecialEffectInfluenceValue.speed >
                                                        bulletSpecialEffect.effectValue ?
                                                        SpecialEffectInfluenceValue.speed :
                                                        bulletSpecialEffect.effectValue;
                break;
            case GameSpecialEffect.SpecialEffect.對攻擊目標造成持續傷害:
                SpecialEffectInfluenceValue.hp = SpecialEffectInfluenceValue.hp >
                                                    bulletSpecialEffect.effectValue ?
                                                    SpecialEffectInfluenceValue.hp :
                                                    bulletSpecialEffect.effectValue;
                //Debug.Log("hp influence");
                break;
            case GameSpecialEffect.SpecialEffect.降低目標的防禦:
                SpecialEffectInfluenceValue.defense = SpecialEffectInfluenceValue.defense >
                                                    bulletSpecialEffect.effectValue ?
                                                    SpecialEffectInfluenceValue.defense :
                                                    bulletSpecialEffect.effectValue;
                break;
            case GameSpecialEffect.SpecialEffect.攻擊時有機會凍住敵人:
                float rate = bulletSpecialEffect.effectValue;
                if(UnityEngine.Random.Range(0f,100f) <= rate)
                {
                    SpecialEffectInfluenceValue.speed = 100f;
                }
                break;
        }

    }
    IEnumerator AddStatus(SpecialEffectInfluenceValue seivalue, 
                            GameSpecialEffect.SpecialEffect effectDescription,
                            float effectValue, float effectKeepTime)
    {
        ChangeSpecialInfluenceValue(seivalue, effectDescription, effectValue, true);
        while(effectKeepTime > 0)
        {
            effectKeepTime -= Time.deltaTime * 1;
            yield return null;
        }
        ChangeSpecialInfluenceValue(seivalue, effectDescription, effectValue, false);
    }
    void ChangeSpecialInfluenceValue(SpecialEffectInfluenceValue seivalue,
                            GameSpecialEffect.SpecialEffect effectDescription,
                            float effectValue, bool add)
    {
        float weight = add ? 1 : -1;
        switch (effectDescription)
        {
            case GameSpecialEffect.SpecialEffect.攻擊時有機會凍住敵人:
                seivalue.speed += 100f * weight;
                break;
        }
    }*/
}

public class Monster : MonsterBase
{
    public Monster(MonsterSettingNew monsterSetting)
    {
        this.monsterName = monsterSetting.name;
        this.speed = monsterSetting.speed;
        this.damage = monsterSetting.damage;
        this.Attribute = monsterSetting.attribute;
        this.HP = monsterSetting.hp;
        this.killBonuse = monsterSetting.killBonus;
        this.SpecialEffect = monsterSetting.specialEffect;
        this.Attribute = monsterSetting.attribute;
        this.elementDropRate = monsterSetting.elementDropRate;
        this.defense = monsterSetting.defense;
    }
    public override float CaculateDamage(float damage, GameAttribute enemyAttribute)
    {
        float finalDamage = damage;
        float weight = 1 + (enemyAttribute.level - this.Attribute.level) * 0.5f;
        switch (enemyAttribute.attribute)
        {
            case GameAttribute.Attribute.水:
                if(this.Attribute.attribute == GameAttribute.Attribute.火)
                {
                    finalDamage *= weight;
                }
                break;
        }
        
        float finalDefense = this.defense - this.SpecialEffectInfluenceValue.defense;
        finalDamage = Mathf.Clamp(finalDamage - finalDefense, 0, finalDamage);
        //finalDamage -= finalDefense;
        return finalDamage;
    }
}


[Serializable]
public class MonsterSettingNew
{
    public float hp;
    public string name;
    public float speed;
    public int damage;
    public GameAttribute attribute;
    public float killBonus;
    public GameSpecialEffect specialEffect;
    public float elementDropRate;
    public float defense;
}
