using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

namespace Battle
{
    public enum ECompareType
    {
        Less, 
        Equal,
        Greate
    }
    public class HpCondition : StateCondition
    {
        public int Hp;
        public ECompareType CompareType;
        public override bool Pass()
        {
            switch (CompareType)
            {
             case ECompareType.Less:
                 break;
             case ECompareType.Equal:
                 break;
             case ECompareType.Greate:
                 break;
            }
            return base.Pass();
        }
    }
}
