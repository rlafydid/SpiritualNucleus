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
        // public GameplayTag state;
        public override bool Pass()
        {
            return true;
        }
    }
}
