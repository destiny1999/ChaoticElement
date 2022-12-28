using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRingController : MonoBehaviour
{
    int count = 0;
    [Tooltip("0 mean the peripheral")]
    [SerializeField] List<BulletController> fireRings;
    [SerializeField] List<float> damages = new List<float>();
    [SerializeField] GameAttribute attribute;
    [SerializeField] List<GameSpecialEffect> specialEffects;
    [SerializeField][ColorUsage(true, true)]Color color;


    // Start is called before the first frame update
    void Start()
    {
        InitializeRingInfo();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeRingsCount(int nums, bool add)
    {
        bool toOne = count == 0 ? true : false;
        int weight = add ? 1 : -1;
        count += nums * weight;
        ChangeRingsInfo();
        if (toOne)
        {
            this.gameObject.SetActive(true);
        }
    }
    void ChangeRingsInfo()
    {
        for(int i = 0; i< fireRings.Count; i++)
        {
            fireRings[i].bulletSetting.SpecialEffect.effectValue = damages[i] * count;
        }
        if(count == 0)
        {
            this.gameObject.SetActive(false);
        }
    }
    void InitializeRingInfo()
    {
        for(int i = 0; i<fireRings.Count; i++)
        {
            fireRings[i].SetBulletInfo(0, attribute, specialEffects[i], 0, color);
            fireRings[i].bulletSetting.used = true;
        }
    }

}