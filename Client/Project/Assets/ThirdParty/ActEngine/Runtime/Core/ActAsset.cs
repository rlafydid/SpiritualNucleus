using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act
{
    [Serializable, CreateAssetMenu(fileName = "Act_XXX_XXX", menuName = "ACT技能表现配置", order = 7)]
    public class ActAsset : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField]
        public float LifeTime = 3.0f;
        [SerializeField, SerializeReference]
        public List<ActBaseClip> Clips = new List<ActBaseClip>();

        [SerializeField, SerializeReference]
        public List<ActBaseEvent> Events = new List<ActBaseEvent>();

        public string Model;

        public void OnAfterDeserialize()
        {
            foreach (var item in Clips)
            {
                item.Asset = this;
                item.ActInstance = null;
            }
        }

        public void OnBeforeSerialize()
        {
        }

        public void CheckLifeTime()
        {
            float maxTime = GetMaxTime();
            if (LifeTime > maxTime)
                LifeTime = maxTime;
        }

        public float GetMaxTime()
        {
            float maxTime = 0;
            foreach (var item in Clips)
            {
                float endTime = item.TriggerTime;
                if (item.Duration > 0)
                    endTime += item.Duration;

                if (endTime > maxTime)
                    maxTime = endTime;
            }

            foreach (var item in Events)
            {
                float endTime = item.TriggerTime;

                if (endTime > maxTime)
                    maxTime = endTime;
            }
            return maxTime;
        }
    }
}
