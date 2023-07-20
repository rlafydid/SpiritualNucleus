using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act
{
    [ActDisplayName("弹道事件", "事件/弹道事件"), Serializable]
    public class ActBulletEvent : ActBaseEvent
    {
        [SerializeField, SerializeReference]
        public ActBaseBulletData BulletData;
    }
}


