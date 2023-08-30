using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class HeroMoveState : HeroState
    {
        public override void Enter()
        {
            base.Enter();
            Debug.Log("Hero Run");
            // owner.PlayAnim("Run");
            GetActor.Entity.GetComponent<MoveComponent>().ToDefaultState();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
            owner.StopMove();
        }
    }
}
