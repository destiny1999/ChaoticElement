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
    void Start()
    {
        monster = new Monster(monsterSetting);
        if (notMove) return;
        nextTargetTransform = GameObject.Find("FirstMoveCheckPoint").gameObject.transform;
    }
    private void Update()
    {
        moveTime += Time.deltaTime * 1;
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
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!dead)
        {
            if (other.transform.CompareTag("bullet") && 
                    !other.GetComponent<BulletController>().bulletSetting.used)
            {
                other.GetComponent<BulletController>().bulletSetting.used = true;
                float damage = other.transform.GetComponent<BulletController>().bulletSetting.damage;
                GameAttribute enemyAttribute = other.transform.GetComponent<BulletController>().
                                                            bulletSetting.Attribute;

                if (other.transform.GetComponent<BulletController>().bulletSetting.
                        SpecialEffect.effect != GameSpecialEffect.SpecialEffect.無特殊效果)
                {
                    monster.DealWithSpecialEffectValue(other.transform.
                        GetComponent<BulletController>().bulletSetting.SpecialEffect);

                    if(monster.SpecialEffectInfluenceValue.hp != 0 && !burning)
                    {
                        StartCoroutine(DealWithBurn());
                    }
                }
                damage = monster.CaculateDamage(damage, enemyAttribute);
                Destroy(other.gameObject);
                ReduceHP(damage);
            }
        }
        
    }
    IEnumerator DealWithBurn()
    {
        burning = true;
        float time = 1;
        while(monster.HP > 0)
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
        }

    }
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