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
    public void SetBulletInfo(float damage, int index, float effectValue, float speed, Color color)
    {
        bulletSetting.damage = damage;
        bulletSetting.effectIndex = index;
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
    public int effectIndex;
    public float effectValue;
    public float speed;
}
