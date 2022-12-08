using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector] public BulletSetting bulletSetting = new BulletSetting();
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
    public void SetBulletInfo(float damage, GameAttribute attribute, GameSpecialEffect effect, 
                                float speed, Color color)
    {
        bulletSetting.damage = damage;
        bulletSetting.SpecialEffect = effect;
        bulletSetting.Attribute = attribute;
        bulletSetting.speed = speed;
        this.GetComponent<Renderer>().material.color = color;

    }
    public void SetTargetEnemy(GameObject target)
    {
        targetEnemy = target;
    }
}
[Serializable]
public class BulletSetting : GameItemInfo
{
    public float damage;
    public float speed;
    public bool used = false;
}
