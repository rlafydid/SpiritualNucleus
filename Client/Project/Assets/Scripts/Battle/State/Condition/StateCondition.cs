using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public enum EActorState
    {

    }

    public class StateCondition :  FSM.Condition
    {
        public EActorState state;
        public override bool Pass()
        {
            
        }
    }
}
