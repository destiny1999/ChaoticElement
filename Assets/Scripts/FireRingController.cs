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
    [SerializeField] List<GameSpecialEffect> ring1Effect;
    [SerializeField] List<GameSpecialEffect> ring2Effect;
    [SerializeField] List<GameSpecialEffect> ring3Effect;
    [SerializeField][ColorUsage(true, true)]Color color;


    // Start is called before the first frame update
    private void Awake()
    {
        
    }
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeRingsCount(int nums, bool add)
    {
        bool toOne = count == 0 ? true : false;
        if (toOne)
        {
            this.gameObject.SetActive(true);
            InitializeRingInfo();
        }
        int weight = add ? 1 : -1;
        count += nums * weight;
        ChangeRingsInfo();
        if(count == 0)
        {
            this.gameObject.SetActive(false);
        }
        
    }
    void ChangeRingsInfo()
    {
        for(int i = 0; i< fireRings.Count; i++)
        {
            var target = fireRings[i].bulletSetting.GetTargetSpecialEffect(GameSpecialEffect.SpecialEffect.對攻擊目標造成持續傷害);

            target.effectValue = damages[i] * count;
        }
        if(count == 0)
        {
            this.gameObject.SetActive(false);
        }
    }
    void InitializeRingInfo()
    {
        print("inital");
        List<List<GameSpecialEffect>> specialEffects = new List<List<GameSpecialEffect>>();
        specialEffects.Add(ring1Effect);
        specialEffects.Add(ring2Effect);
        specialEffects.Add(ring3Effect);
        for(int i = 0; i<fireRings.Count; i++)
        {
            fireRings[i].SetBulletInfo(0, attribute, specialEffects[i], 0, color);
            fireRings[i].bulletSetting.used = true;
        }
    }
}