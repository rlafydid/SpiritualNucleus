using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
            AttributeSystemComponent system = owner.GetComponent<AttributeSystemComponent>();
            float val = 0;
            switch (AttributeType)
            {
                case "hp" :
                    val = owner.GetComponent<AttributesComponent>().hp;
                    break;
            }
            
            switch (CompareType)
            {
             case ECompareType.Less:
                 return val < AttributeValue;
             case ECompareType.LEqual:
                 return val <= AttributeValue;
             case ECompareType.Equal:
                 return Math.Abs(val - AttributeValue) < float.Epsilon;
             case ECompareType.GEqual:
                 return val >= AttributeValue;
             case ECompareType.Greater:
                 return val > AttributeValue;
            }
            return false;
        }
    }
}
