using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Act
{
    [Serializable]
    public class PreviewModelData
    {
        public string Name;
        public GameObject Model;
    }

    [Serializable]
    public class PreviewSceneData
    {
        public string Name;
        public SceneAsset Scene;
        public Vector3 HeroPosition;
        public Vector3 MonsterPosition;
    }

    [Serializable, CreateAssetMenu(fileName = "Act预览数据配置", menuName = "Act其他/Act预览配置文件", order = 8)]
    public class ActPreviewAsset : ScriptableObject
    {
        public List<PreviewModelData> heroList = new List<PreviewModelData>();
        public List<PreviewModelData> monsterList = new List<PreviewModelData>();
        public List<PreviewSceneData> sceneList = new List<PreviewSceneData>();
    }
}


