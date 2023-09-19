using System;
using LKEngine;
using UnityEngine;

namespace Battle
{
    public class MonsterTraceTargetState : MonsterState<MonsterTraceTargetState.StateData>
    {
        public new struct StateData : IStateData
        {
            public Entity entity;
        }
    
        protected override void OnEnter()
        {
            GetActor.TraceTarget(Data.entity);
            GetActor.PlayAnim("Run");
        }

        protected override void OnUpdate()
        {
            if (Vector3.Distance(GetActor.Position, Data.entity.Position) < 2)
            {
                this.GetActor.StopMove();
                this.GetActor.GetComponent<HeroSkillComponent>().UseNormalAbility();
            }
        }
    }

}
