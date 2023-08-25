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
    
        public override void Enter()
        {
            base.Enter();
            GetActor.TraceTarget(Data.entity);
            GetActor.PlayAnim("Run");
        }

        public override void Update()
        {
            base.Update();
            if (Vector3.Distance(GetActor.Position, Data.entity.Position) < 2)
            {
                this.GetActor.GetComponent<HeroSkillComponent>().UseNormalAbility();
            }
        }
    }

}
