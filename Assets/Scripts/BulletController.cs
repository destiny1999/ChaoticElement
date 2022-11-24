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
            case BuildingSetting.BuildingEffect.無特殊效果:
                break;
            case BuildingSetting.BuildingEffect.降低攻擊目標的移動速度:
                bulletSetting.effect = BulletSetting.BulletEffect.緩速;
                break;
            case BuildingSetting.BuildingEffect.對攻擊目標造成持續傷害:
                bulletSetting.effect = BulletSetting.BulletEffect.持續傷害;
                break;
            case BuildingSetting.BuildingEffect.降低目標怪物的防禦:
                bulletSetting.effect = BulletSetting.BulletEffect.降低防禦;
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
        無特殊效果,
        緩速,
        持續傷害,
        降低防禦
    }
}
