using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class HeroIdleState : HeroState
    {
        private JoystickMoveComponent _joytick;
        protected override void OnEnter()
        {
            GetActor.StartMove();
            _joytick = GetActor.GetComponent<JoystickMoveComponent>();

        }
        protected override void OnUpdate()
        {
            if (_joytick.IsMoving)
            {
                ChangeState(ERoleState.Move);
            }
        }
    }
}