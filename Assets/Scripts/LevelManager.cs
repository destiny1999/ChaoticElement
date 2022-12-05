using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] List<LevelSetting> levelSettings;
    [SerializeField] List<LevelSetting> infinityLevelSetting;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// wave will start at 1
    /// </summary>
    /// <param name="wave"></param>
    /// <returns></returns>
    public LevelSetting GetLevelInfo(int wave, bool infinity)
    {
        if (infinity)
        {
            return infinityLevelSetting[0];
        }
        else
        {
            wave--;
            return levelSettings[wave];
        }
    }
    public int GetMaxLevel()
    {
        return levelSettings.Count;
    }
}
[Serializable]
public class LevelSetting
{
    public int monsterNums;
    public GameObject monster;
}