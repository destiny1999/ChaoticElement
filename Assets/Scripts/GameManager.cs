using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] float eachWaveRestTime;
    [SerializeField] GameMode gameMode;
    [SerializeField] GameObject challengeMonster;
    [SerializeField] List<LevelSetting> levelSettings;
    [SerializeField] List<GameObject> players;
    //[SerializeField] List<GameObject> allBuildings;
    [SerializeField] List<EachLevelBuildings> allElementsBuildings;
    [SerializeField] GameObject monsterManager;
    public static GameManager Instance;
    int wave = 0;
    [SerializeField] List<EachLevelCombineInfo> allLevelCombineInfo;
    [SerializeField] List<SpecialEffectSetting> specialEffectSettings;

    [SerializeField] TextMeshProUGUI waveInfoTMP;

    private void Awake()
    {
        Instance = this;
    }
    //[SerializeField] List<LevelSetting> challengeLevelSetting;
    public enum GameMode
    {
        normal,
        challenge,
        fight
    }
    // Start is called before the first frame update
    void Start()
    {
        switch (gameMode)
        {
            case GameMode.normal:
                wave = 1;
                break;
        }
        StartCoroutine(WaveStart());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator WaveStart()
    {
        
        LevelSetting levelSetting = levelSettings[wave];
        ShowWaveInfo(levelSetting.monster);
        for (int i = 0; i<levelSetting.monsterNums; i++)
        {
            yield return (CreateMonster(levelSetting.monster));
        }
    }
    void ShowWaveInfo(GameObject targetMonster)
    {
        
        string monsterName = targetMonster.GetComponent<MonsterController>().
                                GetMonsterSetting().monsterName;

        List<MonsterSetting.SpecialEffect> specialEffects = 
            targetMonster.GetComponent<MonsterController>().GetMonsterSetting().specialEffects;

        string monsterEffect = "";

        for(int i = 0; i< specialEffects.Count; i++)
        {
            switch (specialEffects[i])
            {
                case MonsterSetting.SpecialEffect.�L�S��ĪG:
                    monsterEffect += "�S�������O ";
                    break;
                case MonsterSetting.SpecialEffect.���ݩʧܩ�:
                    monsterEffect += "���ݩʧܩ� ";
                    break;
                case MonsterSetting.SpecialEffect.���ݩʧܩ�:
                    monsterEffect += "���ݩʧܩ� ";
                    break;
                case MonsterSetting.SpecialEffect.���ݩʧܩ�:
                    monsterEffect += "���ݩʧܩ� ";
                    break;
                case MonsterSetting.SpecialEffect.�������ݩʧܩ�:
                    monsterEffect += "�������ݩʧܩ� ";
                    break;
                case MonsterSetting.SpecialEffect.�t�ݩʧܩ�:
                    monsterEffect += "�t�ݩʧܩ� ";
                    break;
                case MonsterSetting.SpecialEffect.���ݩʤ@���@�y:
                    monsterEffect += "���ݩʤ@���@�y ";
                    break;
                case MonsterSetting.SpecialEffect.���ݩʤ@���@�y:
                    monsterEffect += "���ݩʤ@���@�y ";
                    break;
                case MonsterSetting.SpecialEffect.���ݩʤ@���@�y:
                    monsterEffect += "���ݩʤ@���@�y ";
                    break;
            }
        }
        waveInfoTMP.text = monsterName + "\n" + monsterEffect;
        waveInfoTMP.gameObject.SetActive(true);
        StartCoroutine(HideGameObject(waveInfoTMP.gameObject, 5f));
    }
    public IEnumerator HideGameObject(GameObject target, float time)
    {
        while(time > 0)
        {
            time -= Time.deltaTime * 1;
            yield return null;
        }
        target.SetActive(false);
    }
    IEnumerator CreateMonster(GameObject monster)
    {
        GameObject newMonster = GameObject.Instantiate(monster);
        newMonster.SetActive(false);
        int areaIndex = monster.GetComponent<MonsterController>().GetMonsterSetting().areaIndex;
        newMonster.transform.position = players[areaIndex].GetComponent<PlayerController>().
                                            GetMonsterCreatePosition().position;
        newMonster.transform.SetParent(monsterManager.transform);
        newMonster.SetActive(true);
        float time = 0.5f;
        while(time > 0)
        {
            time -= Time.deltaTime * 1;
            yield return null;
        }
    }
    void WaveEnd()
    {

    }
    public void SendKillStatus(MonsterSetting monsterSetting)
    {
        if(monsterSetting.hp <= 0)
        {
            players[monsterSetting.areaIndex].GetComponent<PlayerController>().
                AddMp(monsterSetting.killBonus);
            players[monsterSetting.areaIndex].GetComponent<PlayerController>().
                AddScore(monsterSetting.killBonus);
        }
        else
        {
            players[monsterSetting.areaIndex].GetComponent<PlayerController>().
                SubHP(monsterSetting.punish);
        }
        if(gameMode == GameMode.challenge)
        {

            // create next level monster
            GameObject newPowerfulMonster = Instantiate(challengeMonster);
            newPowerfulMonster.SetActive(false);
            MonsterSetting newMonsterSetting = new MonsterSetting();
            newMonsterSetting.hp = monsterSetting.orignalHP * 1.5f;
            newMonsterSetting.moveSpeed = Mathf.Clamp(monsterSetting.moveSpeed * 1.1f,
                                                       monsterSetting.moveSpeed,
                                                       5f);
            newMonsterSetting.killBonus = monsterSetting.killBonus + 1;
            Transform targetTransform = players[monsterSetting.areaIndex].
                                            GetComponent<PlayerController>().GetMonsterCreatePosition();
            
            newPowerfulMonster.transform.position = targetTransform.position;
            newPowerfulMonster.GetComponent<MonsterController>().
                SetMonsterSetting(newMonsterSetting);
            newPowerfulMonster.SetActive(true);
        }
    }
    
    public GameObject GetBuildingGameObject(int level, int buildingIndex)
    {
        return allElementsBuildings[level].buildings[buildingIndex];
    }
    public GameObject GetSelfPlayer(int areaIndex)
    {
        return players[areaIndex];
    }
    public int GetLevelBuildingNums(int index)
    {
        return allElementsBuildings[index].buildings.Count;
    }
    public GameObject GetNewLevelUpBuilding(int currentLevel, int code1, int code2)
    {
        EachLevelCombineInfo targetLevel = allLevelCombineInfo[currentLevel-1];
        int targetCode = -1;
        bool get = false;
        for(int i = 0; i<targetLevel.eachLevelCombineInfo.Count; i++)
        {
            if (targetLevel.eachLevelCombineInfo[i].code1 == code1)
            {
                for(int j = 0; j < targetLevel.eachLevelCombineInfo[i].canCombineInfo.Count; j++)
                {
                    if (targetLevel.eachLevelCombineInfo[i].canCombineInfo[j].code2 == code2)
                    {
                        targetCode = targetLevel.eachLevelCombineInfo[i].canCombineInfo[j].targetCode;
                        get = true;
                        break;
                    }
                }
                if (get) break;
            }
        }
        GameObject targetBuilding = allElementsBuildings[currentLevel].buildings[targetCode];
        return targetBuilding;
    }
    public SpecialEffectSetting GetSpecialEffectSetting(int statusCode)
    {
        return specialEffectSettings[statusCode];
    }
}
[Serializable]
public class LevelSetting
{
    public int monsterNums;
    public GameObject monster;
}
[Serializable]
public class EachLevelBuildings
{
    public List<GameObject> buildings;
}
[Serializable]
public class CombineInfo
{
    public int code2;
    public int targetCode;
}
[Serializable]
public class CombineForm
{
    public int code1;
    public List<CombineInfo> canCombineInfo;
}
[Serializable]
public class EachLevelCombineInfo
{
    public List<CombineForm> eachLevelCombineInfo;
}
[Serializable]
public class SpecialEffectSetting
{
    // can be buff or nerf
    public int statusCode;
    [HideInInspector] public float effectValue;
    public EffectInfluenceTarget effectInfluenceTarget; // building damage 1
    public EffectShowPosition effectShowPosition; // the buff will show at target's where
    public GameObject effectVisual; // the buff object, just like a blue ring mean slow down speed
    public enum EffectInfluenceTarget
    {
        buildingDamage,
        buildingAttackSpeed,
        bulletSpeed,
        monsterSpeed,
        monsterDefense,
        monsterHp,
        skillCD
    }
    public enum EffectShowPosition
    {
        headTop,
        belowRing
    }
}
[Serializable]
public class SpecialEffectInfluenceValue
{
    public float hp;
    public float defense;
    public float speed;
    public float damage;
    public float attackCDSpeed;

    public void ChangeInfluenceValue(GameSpecialEffect gameSpecialEffect, bool add)
    {
        int weight = add ? 1 : -1;
        switch (gameSpecialEffect.effect)
        {
            case GameSpecialEffect.SpecialEffect.���C�����ؼЪ����ʳt��:
                speed += gameSpecialEffect.effectValue * weight;
                break;
            case GameSpecialEffect.SpecialEffect.�Y�u���񨾿m�𪺧������j:
                attackCDSpeed +=
                    gameSpecialEffect.effectValue * weight * -1;
                break;
            case GameSpecialEffect.SpecialEffect.���ɥؼЪ������ˮ`:
                damage += gameSpecialEffect.effectValue * weight;
                break;
        }
    }
}
[Serializable]
public class GameAttribute
{
    public Attribute attribute;


    public int level;
    public enum Attribute
    {
        ��,
        ��,
        ��,
        ��,
        �t
    }
}
[Serializable]
public class GameSpecialEffect
{
    public SpecialEffect effect;
    public float effectValue;
    public enum SpecialEffect
    {
        �L�S��ĪG,
        ���C�����ؼЪ����ʳt��,
        ������ؼгy������ˮ`,
        �C�����@�U�ؼ��Y�u�ۨ��������j,
        �Y�u���񨾿m�𪺧������j,
        ���C�ؼЪ����m,
        ���ɥؼЪ������ˮ`
    }
    
}
public class GameItemInfo
{
    public GameAttribute Attribute;
    public GameSpecialEffect SpecialEffect;
    public SpecialEffectInfluenceValue SpecialEffectInfluenceValue = new SpecialEffectInfluenceValue();
}