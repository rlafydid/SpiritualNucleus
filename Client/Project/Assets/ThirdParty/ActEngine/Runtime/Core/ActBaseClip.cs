using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act
{
    [Serializable]
    public class ActBaseClip
    {
        [ActDisplayName("触发时间")]
        public float TriggerTime;
        [ActDisplayName("持续时间")]
        public float Duration = 1;
        [ActDisplayName(null)]
        public int Index;
        [ActDisplayName(null)]
        public int Row;
        public ActInstance ActInstance { get; set; }
        [NonSerialized]
        protected bool isDestroted = false;

        protected ActEntity owner { get => ActInstance != null ? ActInstance.Owner : null; }

        protected bool isDebugMode
        {
            get
            {
                return ActInstance != null && ActInstance.IsDebugMode;
            }
        }

        public virtual bool SupportResize { get; } //Timeline里用

        public ActAsset Asset { get; set; }

        public void Reset()
        {
            isDestroted = false;
        }

        public virtual void Preload()
        {

        }

        public virtual bool OnTrigger() { return true; }

        public virtual void Update(float time, float deltaTime)
        {

        }

        public virtual void OnEnable()
        {

        }

        public virtual void OnDisable()
        {

        }

        public virtual void ChangeSpeed(float speed) { }

        public virtual void Destory()
        {
            isDestroted = true;
        }
    }
}
