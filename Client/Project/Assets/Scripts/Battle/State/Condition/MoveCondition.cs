using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class MoveCondition : StateCondition
    {
        public bool isMoving = false;
        public override bool Pass()
        {
            return owner.GetComponent<JoystickMoveComponent>().IsMoving == isMoving;
        }
    }
}

