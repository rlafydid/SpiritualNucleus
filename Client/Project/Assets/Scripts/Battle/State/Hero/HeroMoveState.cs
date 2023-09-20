using System.Collections;
using System.Collections.Generic;
using FSM;
using UnityEngine;

namespace Battle
{
    public class HeroMoveState : HeroState
    {
        private bool _isFasterRun = false;
        protected override void OnEnter()
        {
            Debug.Log("Hero Run");
            // owner.PlayAnim("Run");
            GetActor.Entity.GetComponent<MoveComponent>().ToDefaultState();

            if (fsm.LastState == (int)ERoleState.Evade || GetActor.GetComponent<JoystickMoveComponent>().FastMoving)
            {
                GetActor.GetComponent<JoystickMoveComponent>().StartRunFaster();
            }
        }

        protected override void OnExit()
        {
            // owner.StopMove();
        }
    }
}
