using System;
using System.Collections;
using System.Collections.Generic;
using FSM;
using UnityEngine;

namespace Battle
{
    public class HeroMoveState : HeroState
    {
        private bool _isFasterRun = false;
        private JoystickMoveComponent _joystick;
        protected override void OnEnter()
        {
            Debug.Log("Hero Run");
            // owner.PlayAnim("Run");
            GetActor.Entity.GetComponent<MoveComponent>().ToDefaultState();

            if (fsm.LastState == (int)ERoleState.Evade || GetActor.GetComponent<JoystickMoveComponent>().IsRunFaster)
            {
                _isFasterRun = true;
                GetActor.GetComponent<JoystickMoveComponent>().StartRunFaster();
            }

            _joystick = GetActor.GetComponent<JoystickMoveComponent>();
            _joystick.RegisterMoveStateChanged(OnMoveStateChanged);
            MakeStateTransitionable();
        }

        void OnMoveStateChanged(bool isMove)
        {
            if(!isMove)
                ExitState();
        }

        protected override void OnExit()
        {
            // owner.StopMove();
            _joystick = GetActor.GetComponent<JoystickMoveComponent>();
            _joystick.UnregisterMoveStateChanged(OnMoveStateChanged);
        }
    }
}
