using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public struct DeadStateData : IStateData
    {
        public string anim;
    }
    public class MonsterDeadState : MonsterState
    {
        public override void Enter()
        {
            base.Enter();
            // if(Math.Abs(owner.Position.ToGroundPos().y - owner.Position.y))
            owner.PlayAnim("Dead");
            owner.DontToDefaultAnimation();
            Facade.Battle.ReleaseActor(owner.ID);

        }


        void Destroy()
        {
        }
    }

}

