using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act
{
    [Serializable]
    public class ActBaseEvent
    {
        [ActDisplayName("触发时间")]
        public float TriggerTime;
        [ActDisplayName("事件名称")]
        public string EventName = "触发事件";
        [SerializeField, ActDisplayName(null)]
        int _row;
        public int Row { get => _row; set => _row = value; }
        [ActDisplayName(null)]
        public string GUID;
        public void CreateGUID()
        {
            GUID = System.Guid.NewGuid().ToString();
        }

        public void CheckGUID()
        {
            if (string.IsNullOrEmpty(GUID))
                CreateGUID();
        }
    }
}
