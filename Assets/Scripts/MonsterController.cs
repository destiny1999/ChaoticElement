using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] MonsterSetting monsterSetting;
    Transform nextTargetTransform;
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
        if(monsterSetting.hp <= 0)
        {
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
            float effectIndex = other.transform.GetComponent<BulletController>().bulletSetting.effectIndex;
            float effectValue = other.transform.GetComponent<BulletController>().bulletSetting.effectValue;
            GetHurt(damage);
            Destroy(other.gameObject);
        }
    }
}
[Serializable]
public class MonsterSetting
{
    [HideInInspector]public float orignalHP;
    public float hp;
    public float moveSpeed;
    public int specialEffectCode;
    public int killBonus;
    public int areaIndex;
    public int punish;
}