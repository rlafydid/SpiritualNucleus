using System;
using LKEngine;
using UnityEngine;

namespace Battle
{
    public class MonsterTraceTargetState : MonsterState
    {
        Entity entity;
        public override void SetParameters(object parameters)
        {
            entity = (Entity)parameters;
        }
        public override void Enter()
        {
            base.Enter();
            GetActor.TraceTarget(entity);
            GetActor.PlayAnim("Run");
        }

        public override void Exexute()
        {
            base.Exexute();
            if (Vector3.Distance(GetActor.Position, entity.Position) < 2)
            {
                //owner.TriggetNormalAttack();
                ChangeState(ERoleState.Attack);
            }
        }
    }

}
