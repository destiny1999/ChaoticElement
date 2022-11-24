using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    // Start is called before the first frame update
    public BulletSetting bulletSetting;
    GameObject targetEnemy;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(targetEnemy == null)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetEnemy.transform.position,
                            bulletSetting.speed * Time.deltaTime);
        }
    }
    public void SetBulletInfo(float damage, BuildingSetting.BuildingEffect effect, float effectValue, float speed, Color color)
    {
        bulletSetting.damage = damage;

        switch (effect)
        {
            case BuildingSetting.BuildingEffect.�L�S��ĪG:
                break;
            case BuildingSetting.BuildingEffect.���C�����ؼЪ����ʳt��:
                bulletSetting.effect = BulletSetting.BulletEffect.�w�t;
                break;
            case BuildingSetting.BuildingEffect.������ؼгy������ˮ`:
                bulletSetting.effect = BulletSetting.BulletEffect.����ˮ`;
                break;
            case BuildingSetting.BuildingEffect.���C�ؼЩǪ������m:
                bulletSetting.effect = BulletSetting.BulletEffect.���C���m;
                break;
        }
        bulletSetting.effectValue = effectValue;
        bulletSetting.speed = speed;
        this.GetComponent<Renderer>().material.color = color;
    }
    public void SetTargetEnemy(GameObject target)
    {
        targetEnemy = target;
    }
}
[Serializable]
public class BulletSetting
{
    public float damage;
    public BulletEffect effect;
    public float effectValue;
    public float speed;

    public enum BulletEffect
    {
        �L�S��ĪG,
        �w�t,
        ����ˮ`,
        ���C���m
    }
}
