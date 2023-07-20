using System;
using UnityEngine;

namespace Battle
{
    public class MonsterIdleState : MonsterState
    {
        float t;
        public override void Enter()
        {
            base.Enter();
            t = 0;
            if(owner.IsDead())
            {
                ChangeState(ERoleState.Dead);
            }

            //GetActor.PlayAnim("Default");
        }

        public override void Exexute()
        {
            base.Exexute();
            if (owner.IsDead())
                return;

            t += Time.deltaTime;
            if (t < 1)
                return;

            var target = Facade.Battle.SelectNearestTarget(owner, Facade.Battle.GetHeroActors(), 10);
            if(target != null)
            {
                this.TriggerEvent(ERoleState.TraceTarget, target.Entity);
            }
        }


        public override void Exit()
        {
            base.Exit();
        }
    }
}
