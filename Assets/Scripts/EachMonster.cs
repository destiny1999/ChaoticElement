using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EachMonster : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] MonsterSettingNew monsterSetting;
    Monster monster;
    void Start()
    {
        monster = new Monster(monsterSetting.name, monsterSetting.speed,
                                        monsterSetting.damage, monsterSetting.attribute,
                                        monsterSetting.attributeLevel);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("bullet"))
        {
            float damage = other.transform.GetComponent<BulletController>().bulletSetting.damage;
            GameAttribute enemyAttribute = other.transform.GetComponent<BulletController>().
                                                        bulletSetting.Attribute;

            monster.GetHit(damage, enemyAttribute);
            
            if (other.transform.GetComponent<BulletController>().bulletSetting.SpecialEffect.effect !=
                    GameSpecialEffect.SpecialEffect.無特殊效果)
            {
                monster.DealWithSpecialEffect(other.transform.GetComponent<BulletController>().bulletSetting.SpecialEffect);
            }

            //GetHurt(damage);
            Destroy(other.gameObject);
        }
    }
}

public abstract class MonsterBase : GameItemInfo
{
    public int test;
    string _name;
    float _hp;
    float _speed;
    int _damage;
    public abstract float GetHit(float damage, GameAttribute enemyAttribute);
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
    public Monster(string _name, float _speed, int _damage, GameAttribute.Attribute _attribue, int _attribueLevel)
    {
        this.monsterName = _name;
        this.speed = _speed;
        this.damage = _damage;
        this.Attribute.attribute = _attribue;
        this.Attribute.level = _attribueLevel;
    }
    public override float GetHit(float damage, GameAttribute enemyAttribute)
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
    public string name;
    public float speed;
    public int damage;
    public GameAttribute.Attribute attribute;
    public int attributeLevel;
}