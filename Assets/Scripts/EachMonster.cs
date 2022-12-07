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
    bool dead = false;
    void Start()
    {
        monster = new Monster(monsterSetting);
        nextTargetTransform = GameObject.Find("FirstMoveCheckPoint").gameObject.transform;
    }
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextTargetTransform.position,
                            monster.speed * Time.deltaTime);
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
                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!dead)
        {
            if (other.transform.CompareTag("bullet"))
            {
                float damage = other.transform.GetComponent<BulletController>().bulletSetting.damage;
                GameAttribute enemyAttribute = other.transform.GetComponent<BulletController>().
                                                            bulletSetting.Attribute;

                damage = monster.CaculateDamage(damage, enemyAttribute);

                if (other.transform.GetComponent<BulletController>().bulletSetting.SpecialEffect.effect !=
                        GameSpecialEffect.SpecialEffect.無特殊效果)
                {
                    monster.DealWithSpecialEffect(other.transform.GetComponent<BulletController>().bulletSetting.SpecialEffect);
                }
                Destroy(other.gameObject);
                monster.HP -= damage;
                this.GetComponent<HPMonitorController>().ChangeHpShowValue(monster.HP / monsterSetting.hp);
                if (monster.HP <= 0)
                {
                    dead = true;
                    GameManager.Instance.SendKillStatus(monster);
                    bool drop = UnityEngine.Random.Range(0f, 100f) <= monster.elementDropRate ? true : false;
                    if (drop) GameManager.Instance.CreateDropElement(monster.gameAttribute.attribute,
                                                                        transform.localPosition);
                    Destroy(gameObject);
                }
            }
        }
        
    }
    public MonsterSettingNew GetMonsterSetting()
    {
        return monsterSetting;
    }
}

public abstract class MonsterBase : GameItemInfo
{
    public int test;
    string _name;
    float _hp;
    float _speed;
    int _damage;
    float _killBonuse;
    public abstract float CaculateDamage(float damage, GameAttribute enemyAttribute);
    public float elementDropRate { get; set; }
    GameSpecialEffect _specialEffect;
    GameAttribute _attribute;
    public GameSpecialEffect specialEffect
    {
        get => _specialEffect;
        set => _specialEffect = value;
    }
    public GameAttribute gameAttribute
    {
        get => _attribute;
        set => _attribute = value;
    }
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
    public void DealWithSpecialEffect(GameSpecialEffect _gameSpecialeffect)
    {
        
        switch (_gameSpecialeffect.effect)
        {
            case GameSpecialEffect.SpecialEffect.降低攻擊目標的移動速度:
                SpecialEffectInfluenceValue.speed = this.SpecialEffect.effectValue >
                                                        SpecialEffectInfluenceValue.speed ?
                                                        this.SpecialEffect.effectValue :
                                                        SpecialEffectInfluenceValue.speed;
                break;
            case GameSpecialEffect.SpecialEffect.對攻擊目標造成持續傷害:
                SpecialEffectInfluenceValue.hp = this.SpecialEffect.effectValue >
                                                    SpecialEffectInfluenceValue.hp ?
                                                    this.SpecialEffect.effectValue :
                                                    SpecialEffectInfluenceValue.hp;
                break;
            case GameSpecialEffect.SpecialEffect.降低目標的防禦:
                SpecialEffectInfluenceValue.defense = this.SpecialEffect.effectValue >
                                                    SpecialEffectInfluenceValue.defense ?
                                                    this.SpecialEffect.effectValue :
                                                    SpecialEffectInfluenceValue.defense;
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
        this.specialEffect = monsterSetting.specialEffect;
        this.gameAttribute = monsterSetting.attribute;
        this.elementDropRate = monsterSetting.elementDropRate;
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
}