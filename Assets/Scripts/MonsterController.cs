using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] MonsterSetting monsterSetting;
    Transform nextTargetTransform;
    bool dead = false;
    MonsterSpecialEffectInfluenceValue specialEffectInfluenceValue;
    void Start()
    {
        nextTargetTransform = GameObject.Find("FirstMoveCheckPoint").gameObject.transform;
        monsterSetting.orignalHP = monsterSetting.hp;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextTargetTransform.position,
                            monsterSetting.moveSpeed * Time.deltaTime);
        transform.LookAt(nextTargetTransform);
        if(Vector3.Distance(transform.position, nextTargetTransform.position) <= 0.1f)
        {
            if (!nextTargetTransform.GetComponent<PathNodeController>().CheckLastNode())
            {
                nextTargetTransform =nextTargetTransform.GetComponent<PathNodeController>().
                                        GetNext();
            }
            else
            {
                GameManager.Instance.SendKillStatus(monsterSetting);
                Destroy(gameObject);
            }
        }
    }
    void GetHurt(float value)
    {
        monsterSetting.hp -= value;
        if(monsterSetting.hp <= 0 && !dead)
        {
            dead = true;
            GameManager.Instance.SendKillStatus(monsterSetting);
            Destroy(gameObject);
        }
    }
    public MonsterSetting GetMonsterSetting()
    {
        return monsterSetting;
    }
    public void SetMonsterSetting(MonsterSetting setting)
    {
        monsterSetting = setting;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("bullet"))
        {
            float damage = other.transform.GetComponent<BulletController>().bulletSetting.damage;
            
            if (other.transform.GetComponent<BulletController>().bulletSetting.effect != 
                    BulletSetting.BulletEffect.�L�S��ĪG)
            {
                DealWithSpecialEffect(other.transform.GetComponent<BulletController>().bulletSetting);
            }
            
            GetHurt(damage);
            Destroy(other.gameObject);
        }
    }
    void DealWithSpecialEffect(BulletSetting bulletSetting)
    {
        switch (bulletSetting.effect)
        {
            case BulletSetting.BulletEffect.�w�t:
                specialEffectInfluenceValue.speed = bulletSetting.effectValue >
                                                        specialEffectInfluenceValue.speed ?
                                                        bulletSetting.effectValue :
                                                        specialEffectInfluenceValue.speed;
                break;
            case BulletSetting.BulletEffect.����ˮ`:
                specialEffectInfluenceValue.hp = bulletSetting.effectValue >
                                                    specialEffectInfluenceValue.hp ?
                                                    bulletSetting.effectValue :
                                                    specialEffectInfluenceValue.hp;
                break;
            case BulletSetting.BulletEffect.���C���m:
                specialEffectInfluenceValue.defense = bulletSetting.effectValue >
                                                    specialEffectInfluenceValue.defense ?
                                                    bulletSetting.effectValue :
                                                    specialEffectInfluenceValue.defense;
                break;
        }
        
    }
}
[Serializable]
public class MonsterSetting
{
    [HideInInspector]public float orignalHP;
    public string monsterName;
    public float hp;
    public float moveSpeed;
    public List<SpecialEffect> specialEffects;
    public int killBonus;
    public int areaIndex;
    public int punish;

    public enum SpecialEffect
    {
        �L�S��ĪG,
        ���ݩʧܩ�,
        ���ݩʧܩ�,
        ���ݩʧܩ�,
        �������ݩʧܩ�,
        �t�ݩʧܩ�,
        ���ݩʤ@���@�y,
        ���ݩʤ@���@�y,
        ���ݩʤ@���@�y
    }
}