using System;
using UnityEngine;

namespace Battle
{
    public class MonsterIdleState : MonsterState
    {
        float t;
        protected override void OnEnter()
        {
            t = 0;
            owner.Entity.GameObject.GetComponent<SimpleAnimation>().ReturnToDefaultState = true;
            if(owner.IsDead())
            {
                ChangeState(ERoleState.Dead);
            }

            //GetActor.PlayAnim("Default");
        }

        protected override void OnUpdate()
        {
            if (owner.IsDead())
                return;

            t += Time.deltaTime;
            if (t < 1)
                return;

            var target = Facade.Battle.SelectNearestTarget(owner, Facade.Battle.GetHeroActors(), 10);
            if(target != null)
            {
                var data = new MonsterTraceTargetState.StateData();
                data.entity = target.Entity;
                this.TriggerEvent(ERoleState.TraceTarget, data);
            }
        }


        protected override void OnExit()
        {
        }
    }
}
