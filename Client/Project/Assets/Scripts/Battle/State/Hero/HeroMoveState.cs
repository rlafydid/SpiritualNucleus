using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class HeroMoveState : HeroState
    {
        protected override void OnEnter()
        {
            Debug.Log("Hero Run");
            // owner.PlayAnim("Run");
            GetActor.Entity.GetComponent<MoveComponent>().ToDefaultState();
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnExit()
        {
            owner.StopMove();
        }
    }
}
