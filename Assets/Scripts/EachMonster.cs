using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HPMonitorController))]
public class EachMonster : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] MonsterSetting monsterSetting;
    Monster monster;
    Transform nextTargetTransform;
    public bool dead = false;
    public bool notMove = false;
    public bool boss = false;
    bool burning = false;
    float moveTime = 0;
    bool needToSetAttribute = false;
    [SerializeField]List<Status> statusList = new List<Status>();
    Stack<BulletController> burnEffectStack = new Stack<BulletController>();
    private void Awake()
    {

    }
    void Start()
    {
        monster = new Monster(monsterSetting);
        if (needToSetAttribute)
        {
            SetMonsterAttribute();
        }
        if (notMove) return;
        nextTargetTransform = GameObject.Find("FirstMoveCheckPoint").gameObject.transform;
        
    }
    private void Update()
    {
        //moveTime += Time.deltaTime * 1;

        monster.SpecialEffectInfluenceValue.speed = statusList[0].value;
        //monster.SpecialEffectInfluenceValue.hp = statusList[1].value;
        //print("target hp = " + monster.SpecialEffectInfluenceValue.hp);
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
            // check the target is bullet
            if (other.transform.CompareTag("bullet") && 
                    !other.GetComponent<BulletController>().bulletSetting.used)
            {
                // if not range attack it will not attack other 
                if (!other.GetComponent<BulletController>().rangeAttack)
                {
                    other.GetComponent<BulletController>().bulletSetting.used = true;
                }

                // get the bullet's damage
                float damage = other.transform.GetComponent<BulletController>().bulletSetting.damage;
                
                // check the miracle
                if(other.GetComponent<BulletController>().bulletSetting
                    .GetTargetSpecialEffect(GameSpecialEffect.SpecialEffect.攻擊時有機會雙倍傷害) != null)
                {
                    //print("before miracle = " + damage);
                    damage = UnityEngine.Random.Range(0, 2) == 1 ? damage * 2 : damage;
                    //print("after miracle = " + damage);
                }

                //
                GameAttribute enemyAttribute = other.transform.GetComponent<BulletController>().
                                                            bulletSetting.Attribute;
                for(int i = 0; i< other.transform.GetComponent<BulletController>().bulletSetting.
                        SpecialEffects.Count; i++)
                {
                    if (other.transform.GetComponent<BulletController>().bulletSetting.
                        SpecialEffects[i].effect != GameSpecialEffect.SpecialEffect.無特殊效果)
                    {
                        DealWithStatus(other.transform.
                                        GetComponent<BulletController>().bulletSetting.SpecialEffects[i]);
                        //print(monster.SpecialEffectInfluenceValue.hp);
                        //print(burning);
                        if (monster.SpecialEffectInfluenceValue.hp != 0 && !burning)
                        {
                            StartCoroutine(DealWithBurn());
                        }
                    }
                }
                
                damage = monster.CaculateDamage(damage, enemyAttribute);
                if(!boss && other.GetComponent<BulletController>().bulletSetting
                    .GetTargetSpecialEffect(GameSpecialEffect.SpecialEffect.攻擊時有機率秒殺小怪)
                    != null)
                {
                    float value = other.GetComponent<BulletController>().bulletSetting
                    .GetTargetSpecialEffect(GameSpecialEffect.SpecialEffect.攻擊時有機率秒殺小怪)
                    .effectValue;
                    bool dead = UnityEngine.Random.Range(0f, 100f) <= value ? true : false;
                    if (dead)
                    {
                        //print("dead");
                        damage = monster.HP;
                    }
                }
                if (!other.GetComponent<BulletController>().rangeAttack)
                {
                    Destroy(other.gameObject);
                }
                //print("damage = " + damage);
                ReduceHP(damage);
            }
            else if(other.transform.CompareTag("bullet") &&
                      other.GetComponent<BulletController>().bulletSetting.Attribute.attribute
                      == GameAttribute.Attribute.火 &&
                      other.GetComponent<BulletController>().bulletSetting.Attribute.level == 5)
            {
                burnEffectStack.Push(other.GetComponent<BulletController>());
                var target = burnEffectStack.Peek().bulletSetting.GetTargetSpecialEffect(GameSpecialEffect.SpecialEffect.對攻擊目標造成持續傷害);
                //statusList[1].value = burnEffectStack.Peek().bulletSetting.SpecialEffects[0].effectValue;
                statusList[1].value = target.effectValue;
                //print(statusList[1].value);
                //print(monster.SpecialEffectInfluenceValue.hp);
                StartCoroutine(DealWithBurn());
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "bullet" &&
            other.GetComponent<BulletController>().bulletSetting.Attribute.attribute
                      == GameAttribute.Attribute.火 &&
                      other.GetComponent<BulletController>().bulletSetting.Attribute.level == 5)
        {
            burnEffectStack.Pop();
            if(burnEffectStack.Count > 0)
            {
                var target = burnEffectStack.Peek().bulletSetting.GetTargetSpecialEffect(GameSpecialEffect.SpecialEffect.對攻擊目標造成持續傷害);
                statusList[1].value = target.effectValue;
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
                //print("into hp");
                if (bulletSpecialEffect.effectLevel == 5) break;
                newStatus.target = Status.InfluenceStatus.hp;
                //print(bulletSpecialEffect.effectValue);
                if (statusList[1].value <= bulletSpecialEffect.effectValue)
                {
                    statusList[1].value = bulletSpecialEffect.effectValue;
                    monster.SpecialEffectInfluenceValue.hp = statusList[1].value;

                    if (statusList[1].time == 0 )
                    {
                        StartCoroutine(ReduceStatusRemainTime(1, newStatus));
                    }
                    else
                    {
                        statusList[1].time = newStatus.time;
                    }
                    
                }
                break;
            case GameSpecialEffect.SpecialEffect.降低目標的防禦:
                //newStatus.target = Status.InfluenceStatus.defense;
                if(bulletSpecialEffect.effectLevel < 4)
                {
                    statusList[2].value += bulletSpecialEffect.effectValue;
                }
                break;
            case GameSpecialEffect.SpecialEffect.減半目標防禦:
                if (statusList[2].value == 0)
                {
                    statusList[2].value = monster.defense / 2;
                }
                else
                {
                    statusList[2].value += (monster.defense - statusList[2].value) / 2f;
                }
                if (statusList[2].value >=
                        monster.defense * (100 - bulletSpecialEffect.effectValue) / 100f)
                {
                    statusList[2].value = monster.defense;
                }
                break;
        }
    }
    void UseLevel5Ice(Status status)
    {
        int times = 0;
        for(int i = 0; i<5; i++)
        {
            if (UnityEngine.Random.Range(0f, 100f) <= 50f)
            {
                times++;
                //print("success");
            }
        }
        if (statusList[0].value <= times * 20f)
        {
            statusList[0].value = times * 20f;
            //print(statusList[0].value);
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
        //print("reduce time");
        statusList[targetIndex].time = status.time;
        //print("time = " + statusList[targetIndex].time);
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
        monster.SpecialEffectInfluenceValue.hp = statusList[1].value;
        while(monster.SpecialEffectInfluenceValue.hp > 0)
        {
            monster.SpecialEffectInfluenceValue.hp = statusList[1].value;
            //print("keep burn");
            time -= Time.deltaTime * 1;
            if(time <= 0)
            {
                //print("reduce " + monster.SpecialEffectInfluenceValue.hp);
                ReduceHP(monster.SpecialEffectInfluenceValue.hp);
                time = 1;
            }
            yield return null;
        }
    }
    void ReduceHP(float damage)
    {
        if (dead) return;
        //print("damage = " + damage);
        monster.HP -= damage;
        this.GetComponent<HPMonitorController>().ChangeHpShowValue(monster.HP / monsterSetting.hp);
        if (monster.HP <= 0)
        {
            dead = true;
            
            bool drop = UnityEngine.Random.Range(0f, 100f) <= monster.elementDropRate ? true : false;
            if (drop) GameManager.Instance.CreateDropElement(monster.Attribute.attribute,
                                                                transform.localPosition);
            GameManager.Instance.CreateEarnMoney(monster.killBonuse, transform.localPosition);
            GameManager.Instance.SendKillStatus(monster);
            Destroy(gameObject);
        }
    }
    public void SetMonsterAttribute()
    {
        if(monster.Attribute.attribute == GameAttribute.Attribute.隨機)
        {
            monster.Attribute.attribute = GetRandomAttribute();
        }
        switch (monster.Attribute.attribute)
        {
            case GameAttribute.Attribute.水:
                this.transform.Find("MonsterTest").GetComponent<MeshRenderer>().
                    material.color = Color.blue;
                break;
            case GameAttribute.Attribute.火:
                this.transform.Find("MonsterTest").GetComponent<MeshRenderer>().
                    material.color = Color.red;
                break;
            case GameAttribute.Attribute.風:
                this.transform.Find("MonsterTest").GetComponent<MeshRenderer>().
                    material.color = Color.green;
                break;
            case GameAttribute.Attribute.光:
                this.transform.Find("MonsterTest").GetComponent<MeshRenderer>().
                    material.color = Color.yellow;
                break;
            case GameAttribute.Attribute.暗:
                this.transform.Find("MonsterTest").GetComponent<MeshRenderer>().
                    material.color = Color.black;
                break;
        }
    }
    GameAttribute.Attribute GetRandomAttribute()
    {
        GameAttribute.Attribute attribute = GameAttribute.Attribute.無;
        switch (UnityEngine.Random.Range(0, 5))
        {
            case 0:
                attribute = GameAttribute.Attribute.水;
                break;
            case 1:
                attribute = GameAttribute.Attribute.火;
                break;
            case 2:
                attribute = GameAttribute.Attribute.風;
                break;
            case 3:
                attribute = GameAttribute.Attribute.光;
                break;
            case 4:
                attribute = GameAttribute.Attribute.暗;
                break;
        }
        return attribute;
    }
    public MonsterSetting GetMonsterSetting()
    {
        return monsterSetting;
    }
    public void SetNeedToSetAttribute()
    {
        needToSetAttribute = true;
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
}

public class Monster : MonsterBase
{
    public Monster(MonsterSetting monsterSetting)
    {
        this.monsterName = monsterSetting.name;
        this.speed = monsterSetting.speed;
        this.damage = monsterSetting.damage;
        this.Attribute = monsterSetting.attribute;
        this.HP = monsterSetting.hp;
        this.killBonuse = monsterSetting.killBonus;
        this.SpecialEffects = monsterSetting.specialEffect;
        this.Attribute = monsterSetting.attribute;
        this.elementDropRate = monsterSetting.elementDropRate;
        this.defense = monsterSetting.defense;
    }
    public override float CaculateDamage(float damage, GameAttribute bulletAttribute)
    {
        float finalDamage = damage;
        float weight = 1 + (bulletAttribute.level - this.Attribute.level) * 0.5f;
        switch (bulletAttribute.attribute)
        {
            case GameAttribute.Attribute.水:
                if(this.Attribute.attribute == GameAttribute.Attribute.火)
                {
                    finalDamage *= weight;
                }
                break;
            case GameAttribute.Attribute.火:
                if (this.Attribute.attribute == GameAttribute.Attribute.風)
                {
                    finalDamage *= weight;
                }
                break;
            case GameAttribute.Attribute.風:
                if (this.Attribute.attribute == GameAttribute.Attribute.水)
                {
                    finalDamage *= weight;
                }
                break;
            case GameAttribute.Attribute.光:
                if (this.Attribute.attribute == GameAttribute.Attribute.暗)
                {
                    finalDamage *= weight / 0.5f * 2f;
                }
                break;
            case GameAttribute.Attribute.暗:
                if (this.Attribute.attribute != GameAttribute.Attribute.光 &&
                    this.Attribute.attribute != GameAttribute.Attribute.暗)
                {
                    finalDamage *= weight / 0.5f * 0.25f;
                }
                else if(this.Attribute.attribute == GameAttribute.Attribute.光)
                {
                    finalDamage *= weight / 0.5f * 2f;
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
public class MonsterSetting
{
    public float hp;
    public string name;
    public float speed;
    public int damage;
    public GameAttribute attribute;
    public float killBonus;
    public List<GameSpecialEffect> specialEffect;
    public float elementDropRate;
    public float defense;
}
