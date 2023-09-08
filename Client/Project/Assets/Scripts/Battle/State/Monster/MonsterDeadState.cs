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
            owner.PlayAnim("Dead");
            owner.Entity.GameObject.GetComponent<SimpleAnimation>().ReturnToDefaultState = false;
            TimerMod.Delay(2f, Destroy);
        }


        void Destroy()
        {
            Facade.Battle.ReleaseActor(owner.ID);
        }
    }

}

