using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EGenerateType
{
    Hero,
    Monster
}

[Serializable]
public class PointConfig
{
    public long id;
    public EGenerateType generateType;
    public int count;
    public Vector3 point;
    public float radius;
}

[CreateAssetMenu(menuName = "配置/战斗场景/关卡配置文件", fileName = "LevelConfig.asset"), Serializable]
public class SceneConfig : ScriptableObject
{
    public long id;
    public string sceneName;
    public List<PointConfig> pointConfigs; 
}
