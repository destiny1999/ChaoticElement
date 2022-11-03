using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] float eachWaveRestTime;
    [SerializeField] GameMode gameMode;
    [SerializeField] GameObject challengeMonster;
    [SerializeField] List<LevelSetting> levelSettings;
    [SerializeField] List<GameObject> players;
    public static GameManager Instance;
    int wave = 0;
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
        for(int i = 0; i<levelSetting.monsterNums; i++)
        {
            yield return (CreateMonster(levelSetting.monster));
        }
    }
    IEnumerator CreateMonster(GameObject monster)
    {
        GameObject newMonster = GameObject.Instantiate(monster);
        newMonster.SetActive(false);
        int areaIndex = monster.GetComponent<MonsterController>().GetMonsterSetting().areaIndex;
        newMonster.transform.position = players[areaIndex].GetComponent<PlayerController>().
                                            GetMonsterCreatePosition().position;
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
}
[Serializable]
public class LevelSetting
{
    public int monsterNums;
    public GameObject monster;
}
