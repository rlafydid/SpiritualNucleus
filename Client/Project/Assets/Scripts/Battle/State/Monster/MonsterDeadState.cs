using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{

    public class MonsterDeadState : MonsterState
    {
        public override void Enter()
        {
            base.Enter();
            owner.PlayAnim("Dead");
            TimerMod.Delay(2f, Destroy);
        }


        void Destroy()
        {
            Facade.Battle.ReleaseActor(owner.ID);
        }
    }

}

