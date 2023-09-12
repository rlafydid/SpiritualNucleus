using System.Collections;
using System.Collections.Generic;
using AttributeSystem.Components;
using Battle;
using UnityEngine;

namespace Battle
{
    public enum ECompareType
    {
        Less,//小于
        LEqual, //小于等于
        Equal, //等于
        GEqual, //大于党羽
        Greater //大于
    }
    public class AttributeCondition : StateCondition
    {
        public string AttributeType;
        public ECompareType CompareType;
        public float AttributeValue;
        public override bool Pass()
        {
            AttributeSystemComponent system = owner.Entity.GameObject.GetComponent<AttributeSystemComponent>();
            switch (CompareType)
            {
             case ECompareType.Less:
                 break;
             case ECompareType.Equal:
                 break;
             case ECompareType.Greater:
                 break;
            }
            return base.Pass();
        }
    }
}
