using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act
{
    [ActDisplayName("伤害事件", "事件/伤害事件", 1), Serializable]
    public class ActDamageEvent : ActBaseEvent
    {
        [ActDisplayName("攻击范围")]
        public float AttackRange;
    }
}
