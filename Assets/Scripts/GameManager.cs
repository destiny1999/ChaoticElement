using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(LevelManager))]
public class GameManager : MonoBehaviour
{
    [SerializeField] float eachWaveRestTime;
    [SerializeField] GameMode gameMode;
    [SerializeField] GameObject challengeMonster;

    [SerializeField] List<GameObject> players;
    //[SerializeField] List<GameObject> allBuildings;
    [SerializeField] List<EachLevelBuildings> allElementsBuildings;
    [SerializeField] GameObject monsterManager;
    public static GameManager Instance;
    int wave = 0;
    [SerializeField] List<EachLevelCombineInfo> allLevelCombineInfo;
    [SerializeField] List<SpecialEffectSetting> specialEffectSettings;

    [SerializeField] TextMeshProUGUI waveInfoTMP;
    LevelManager levelManager;
    int waveMonsters = 0;
    int maxWave = 0;
    [SerializeField] List<GameObject> dropElements;
    [SerializeField] GameObject buildingManager;

    public float GameExecuteSpeed = 1f;
    private void Awake()
    {
        Instance = this;
        levelManager = this.GetComponent<LevelManager>();
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
        maxWave = levelManager.GetMaxLevel();
        StartCoroutine(NextWave());
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = GameExecuteSpeed;
    }
    IEnumerator NextWave()
    {
        if(wave > maxWave)
        {
            print("completed");
            yield break;
        }
        LevelSetting levelSetting = new LevelSetting();
        
        if(gameMode == GameMode.normal)
        {
            levelSetting = levelManager.GetLevelInfo(wave, false);
        }
        else
        {
            levelSetting = levelManager.GetLevelInfo(0, true);
        }
        waveMonsters = levelSetting.monsterNums;
        ShowWaveInfo(levelSetting.monster);
        for (int i = 0; i<levelSetting.monsterNums; i++)
        {
            yield return (CreateMonster(levelSetting.monster));
        }
    }
    void ShowWaveInfo(GameObject targetMonster)
    {
        MonsterSettingNew monsterSetting = targetMonster.GetComponent<EachMonster>().GetMonsterSetting();
        string monsterName = monsterSetting.name;

        /*
        List<MonsterSetting.SpecialEffect> specialEffects = 
            targetMonster.GetComponent<MonsterController>().GetMonsterSetting().specialEffects;
        */
        string monsterEffect = "";
        switch (monsterSetting.specialEffect.effect)
        {
            case GameSpecialEffect.SpecialEffect.無特殊效果:
                monsterEffect += "沒有任何能力 ";
                break;
            case GameSpecialEffect.SpecialEffect.水屬性抗性:
                monsterEffect += "水屬性抗性 ";
                break;
            case GameSpecialEffect.SpecialEffect.火屬性抗性:
                monsterEffect += "火屬性抗性 ";
                break;
            case GameSpecialEffect.SpecialEffect.風屬性抗性:
                monsterEffect += "風屬性抗性 ";
                break;
            case GameSpecialEffect.SpecialEffect.水火風屬性抗性:
                monsterEffect += "水火風屬性抗性 ";
                break;
            case GameSpecialEffect.SpecialEffect.暗屬性抗性:
                monsterEffect += "暗屬性抗性 ";
                break;
            case GameSpecialEffect.SpecialEffect.水屬性一擊護頓:
                monsterEffect += "水屬性一擊護頓 ";
                break;
            case GameSpecialEffect.SpecialEffect.火屬性一擊護頓:
                monsterEffect += "火屬性一擊護頓 ";
                break;
            case GameSpecialEffect.SpecialEffect.風屬性一擊護頓:
                monsterEffect += "風屬性一擊護頓 ";
                break;
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
        //int areaIndex = monster.GetComponent<MonsterController>().GetMonsterSetting().areaIndex;
        newMonster.transform.position = players[0].GetComponent<PlayerController>().
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
    public void SendKillStatus(Monster monster)
    {
        if(monster.HP <= 0)
        {
            players[0].GetComponent<PlayerController>().
                AddMp(monster.killBonuse);
            players[0].GetComponent<PlayerController>().
                AddScore(wave);
        }
        else
        {
            players[0].GetComponent<PlayerController>().
                SubHP(monster.damage);
        }
        waveMonsters--;
        if(waveMonsters == 0)
        {
            wave++;
            StartCoroutine(NextWave());
        }
        /*
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
        }*/
    }
    public void CreateDropElement(GameAttribute.Attribute attribute, Vector3 dropPosition)
    {
        GameObject dropElement = null;
        switch (attribute)
        {
            case GameAttribute.Attribute.無:
                dropElement = Instantiate(dropElements[0]);
                break;
            case GameAttribute.Attribute.水:
                dropElement = Instantiate(dropElements[1]);
                break;
            case GameAttribute.Attribute.火:
                dropElement = Instantiate(dropElements[2]);
                break;
            case GameAttribute.Attribute.風:
                dropElement = Instantiate(dropElements[3]);
                break;
            case GameAttribute.Attribute.光:
                dropElement = Instantiate(dropElements[4]);
                break;
            case GameAttribute.Attribute.暗:
                dropElement = Instantiate(dropElements[5]);
                break;
        }
        dropElement.transform.position = dropPosition;
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
    /// <summary>
    /// the nums mean how many power the magic pet this attribute had
    /// </summary>
    /// <param name="attribute"></param>
    /// <param name="nums"></param>
    public void SetMagicPetBuff(GameAttribute.Attribute attribute, int nums)
    {
        BuildingController[] buildings = buildingManager.transform.GetComponentsInChildren<BuildingController>();
        //print(buildings.Length);
        for(int i = 0; i<buildings.Length; i++)
        {
            if (buildings[i].buildingSetting.Attribute.attribute == attribute)
            {
                buildings[i].AddMagicPetBuff(nums);
            }
            
        }
    }
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
            case GameSpecialEffect.SpecialEffect.降低攻擊目標的移動速度:
                speed += gameSpecialEffect.effectValue * weight;
                break;
            case GameSpecialEffect.SpecialEffect.縮短附近防禦塔的攻擊間隔:
                attackCDSpeed +=
                    gameSpecialEffect.effectValue * weight * -1;
                break;
            case GameSpecialEffect.SpecialEffect.提升目標的攻擊傷害:
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
        無,
        水,
        火,
        風,
        光,
        暗
    }
}
[Serializable]
public class GameSpecialEffect
{
    public SpecialEffect effect;
    public float effectValue;
    public enum SpecialEffect
    {
        無特殊效果,
        降低攻擊目標的移動速度,
        對攻擊目標造成持續傷害,
        攻擊後縮短自身攻擊間隔,
        縮短附近防禦塔的攻擊間隔,
        降低目標的防禦,
        提升目標的攻擊傷害,
        水屬性抗性,
        火屬性抗性,
        風屬性抗性,
        水火風屬性抗性,
        暗屬性抗性,
        水屬性一擊護頓,
        火屬性一擊護頓,
        風屬性一擊護頓,
    }
    
}
public class GameItemInfo
{
    public GameAttribute Attribute;
    public GameSpecialEffect SpecialEffect;
    public SpecialEffectInfluenceValue SpecialEffectInfluenceValue = new SpecialEffectInfluenceValue();
}