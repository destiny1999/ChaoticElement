using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] GameObject menuObject;
    public float GameExecuteSpeed = 1f;
    float gameExecuteSpeedTemp = 1f;
    [SerializeField] GameObject moneyObject;
    public Transform allAttackBulletCreatePosition;
    public GameObject allAttackCenterTarget;

    [SerializeField] FireRingController fireRingController;

    GameAttribute waveAttribute = null;
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
        gameExecuteSpeedTemp = GameExecuteSpeed;
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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GameExecuteSpeed = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GameExecuteSpeed = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GameExecuteSpeed = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GameExecuteSpeed = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            GameExecuteSpeed = 5;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuObject.SetActive(!menuObject.activeSelf);
            if (menuObject.activeSelf) 
            {
                gameExecuteSpeedTemp = GameExecuteSpeed;
                GameExecuteSpeed = 0;
            }
            else
            {
                GameExecuteSpeed = gameExecuteSpeedTemp;
                gameExecuteSpeedTemp = GameExecuteSpeed;
            }
        }
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
        waveAttribute = null;
    }
    void ShowWaveInfo(GameObject targetMonster)
    {
        MonsterSetting monsterSetting = targetMonster.GetComponent<EachMonster>().GetMonsterSetting();
        string monsterName = monsterSetting.name;

        string monsterEffect = "";
        for(int i = 0; i < monsterSetting.specialEffect.Count; i++)
        {
            switch (monsterSetting.specialEffect[i].effect)
            {
                case GameSpecialEffect.SpecialEffect.�L�S��ĪG:
                    monsterEffect += "�S�������O ";
                    break;
                case GameSpecialEffect.SpecialEffect.���ݩʧܩ�:
                    monsterEffect += "���ݩʧܩ� ";
                    break;
                case GameSpecialEffect.SpecialEffect.���ݩʧܩ�:
                    monsterEffect += "���ݩʧܩ� ";
                    break;
                case GameSpecialEffect.SpecialEffect.���ݩʧܩ�:
                    monsterEffect += "���ݩʧܩ� ";
                    break;
                case GameSpecialEffect.SpecialEffect.�������ݩʧܩ�:
                    monsterEffect += "�������ݩʧܩ� ";
                    break;
                case GameSpecialEffect.SpecialEffect.�t�ݩʧܩ�:
                    monsterEffect += "�t�ݩʧܩ� ";
                    break;
                case GameSpecialEffect.SpecialEffect.���ݩʤ@���@�y:
                    monsterEffect += "���ݩʤ@���@�y ";
                    break;
                case GameSpecialEffect.SpecialEffect.���ݩʤ@���@�y:
                    monsterEffect += "���ݩʤ@���@�y ";
                    break;
                case GameSpecialEffect.SpecialEffect.���ݩʤ@���@�y:
                    monsterEffect += "���ݩʤ@���@�y ";
                    break;
            }
        }

        if (monsterName.Contains("X"))
        {
            waveAttribute = new GameAttribute();
            switch (UnityEngine.Random.Range(0, 5))
            {
                case 0:
                    waveAttribute.attribute = GameAttribute.Attribute.��;
                    monsterName = monsterName.Replace('X', '��');
                    break;
                case 1:
                    waveAttribute.attribute = GameAttribute.Attribute.��;
                    monsterName = monsterName.Replace('X', '��');
                    break;
                case 2:
                    waveAttribute.attribute = GameAttribute.Attribute.��;
                    monsterName = monsterName.Replace('X', '��');
                    break;
                case 3:
                    waveAttribute.attribute = GameAttribute.Attribute.��;
                    monsterName = monsterName.Replace('X', '��');
                    break;
                case 4:
                    waveAttribute.attribute = GameAttribute.Attribute.�t;
                    monsterName = monsterName.Replace('X', '�t');
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
        //int areaIndex = monster.GetComponent<MonsterController>().GetMonsterSetting().areaIndex;
        newMonster.transform.position = players[0].GetComponent<PlayerController>().
                                            GetMonsterCreatePosition().position;
        newMonster.transform.SetParent(monsterManager.transform);
        if(waveAttribute != null)
        {
            newMonster.GetComponent<EachMonster>().GetMonsterSetting().attribute.attribute =
                waveAttribute.attribute;
            newMonster.GetComponent<EachMonster>().SetNeedToSetAttribute();
        }
        
        newMonster.SetActive(true);
        float time = 1f;
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
        
    }
    public void CreateEarnMoney(float value, Vector3 position)
    {
        GameObject newMoneyObject = Instantiate(moneyObject);
        newMoneyObject.transform.position = position;
        newMoneyObject.GetComponent<TextMeshPro>().text = $"+{value}";
    }
    public void CreateDropElement(GameAttribute.Attribute attribute, Vector3 dropPosition)
    {
        GameObject dropElement = null;
        switch (attribute)
        {
            case GameAttribute.Attribute.�L:
                int randomElement = UnityEngine.Random.Range(0, dropElements.Count);
                dropElement = Instantiate(dropElements[randomElement]);
                break;
            case GameAttribute.Attribute.��:
                dropElement = Instantiate(dropElements[1]);
                break;
            case GameAttribute.Attribute.��:
                dropElement = Instantiate(dropElements[2]);
                break;
            case GameAttribute.Attribute.��:
                dropElement = Instantiate(dropElements[3]);
                break;
            case GameAttribute.Attribute.��:
                dropElement = Instantiate(dropElements[4]);
                break;
            case GameAttribute.Attribute.�t:
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
    public GameObject GetParticularBuilding(int level, int code)
    {
        //print("level = " + level + " code = " + code);
        return allElementsBuildings[level].buildings[code];
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
        //print(currentLevel + " " + targetCode);
        if (currentLevel == 1) targetCode = code1; 
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
    public void CallChangeRingsCount(int nums, bool add)
    {
        fireRingController.ChangeRingsCount(nums, add);
    }


    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Exit()
    {
        Application.Quit();
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
    public float extraDamageRate;
    public float extraDamageWeight;

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
public class Status
{
    public  InfluenceStatus target;
    public float value;
    public float time;

    public enum InfluenceStatus
    {
        hp,
        speed,
        defense
    }
}

[Serializable]
public class GameAttribute
{
    public Attribute attribute;


    public int level;
    public enum Attribute
    {
        �L,
        ��,
        ��,
        ��,
        ��,
        �t,
        �H��
    }
}
[Serializable]
public class GameSpecialEffect
{
    public SpecialEffect effect;
    public float effectValue;
    public float effectKeepTime;
    public float effectLevel;
    public enum SpecialEffect
    {
        �L�S��ĪG,
        ���C�����ؼЪ����ʳt��,
        ������ؼгy������ˮ`,
        �������Y�u�ۨ��������j,
        �����ᴣ�ɦۨ������ˮ`,
        �����@�w���ƫ��z���ˮ`,
        �Y�u���񨾿m�𪺧������j,
        ���C�ؼЪ����m,
        ��b�ؼШ��m,
        �����ɦ����v����p��,
        ���ɥؼЪ������ˮ`,
        ���ݩʧܩ�,
        ���ݩʧܩ�,
        ���ݩʧܩ�,
        �������ݩʧܩ�,
        �t�ݩʧܩ�,
        ���ݩʤ@���@�y,
        ���ݩʤ@���@�y,
        ���ݩʤ@���@�y,
        �����ɦ����|���ĤH,
        �����ɦ����|�����ˮ`
    }
    
}
public class GameItemInfo
{
    public GameAttribute Attribute;
    public List<GameSpecialEffect> SpecialEffects;
    public SpecialEffectInfluenceValue SpecialEffectInfluenceValue = new SpecialEffectInfluenceValue();

    public GameSpecialEffect GetTargetSpecialEffect(GameSpecialEffect.SpecialEffect targetEffect)
    {
        bool get = false;
        GameSpecialEffect target = new GameSpecialEffect();
        for (int i = 0; i < SpecialEffects.Count; i++)
        {
            if (SpecialEffects[i].effect == targetEffect)
            {
                target = SpecialEffects[i];
                get = true;
                break;
            }
        }
        if (get)
        {
            return target;
        }
        else
        {
            return null;
        }
    }
}