using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "配置/战斗场景/章节", fileName = "ChapterConfig.asset"), Serializable]
public class ChapterConfig : ScriptableObject
{
   public List<SceneConfig> configs;
}
