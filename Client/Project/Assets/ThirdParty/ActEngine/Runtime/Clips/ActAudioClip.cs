using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act
{
    [ActDisplayName("音效", "播放音效"), Serializable]
    public class ActAudioClip : ActBaseClip
    {
        [ActDisplayName("音效表ID")]
        public string configId;
        [ActDisplayName("音效Sheet")]
        public string cueSheet;
        [ActDisplayName("音效名")]
        public string cueName;

        public override bool SupportResize => false;

        public override void Preload()
        {
            base.Preload();
        }

        public override bool OnTrigger()
        {
            //if (string.IsNullOrEmpty(cueSheet) || string.IsNullOrEmpty(cueName))
            //{
            //    Debug.LogError($"{ActInstance.ActName}中请配置音效ID");
            //    return false;
            //}
            Facade.Externals.PlayAudio(configId, cueSheet, cueName);
            return true;
        }


        public override void OnDisable()
        {
            base.OnDisable();
        }
    }
}

