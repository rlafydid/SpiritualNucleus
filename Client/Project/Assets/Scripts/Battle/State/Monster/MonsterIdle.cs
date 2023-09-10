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
            owner.Entity.GameObject.GetComponent<SimpleAnimation>().ReturnToDefaultState = true;
            if(owner.IsDead())
            {
                ChangeState(ERoleState.Dead);
            }

            //GetActor.PlayAnim("Default");
        }

        public override void Update()
        {
            base.Update();
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


        public override void Exit()
        {
            base.Exit();
        }
    }
}
