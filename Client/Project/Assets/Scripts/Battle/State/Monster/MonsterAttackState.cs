using System;
using UnityEngine;

namespace Battle
{
    public class MonsterAttackState : MonsterState
    {
        SkillUnit triggerSkill;
        SceneActorController target;
        float maxDistance = 3f;

        public override void Enter()
        {
            owner.StopMove();
            triggerSkill = GetActor.GetComponent<NormalAttacksComponent>().FindRandomSkill();
            if (target == null || Vector3.Distance(owner.Position, target.Position) > maxDistance)
            {
                var monsters = Facade.Battle.GetMonsters();
                target = Facade.Battle.SelectNearestTarget(owner, monsters, maxDistance);
            }

            if (target != null)
            {
                Vector3 lookAtPos = target.Position;
                lookAtPos.y = owner.Position.y;
                owner.Entity.LookAt(lookAtPos);
            }

            triggerSkill.Finish = Finish;
            Facade.Skill.TriggerSkill(triggerSkill);
        }


        public override void Exexute()
        {

        }

        void Finish(SkillUnit unit)
        {
            this.ChangeState(ERoleState.Idle);
        }

        public override void Exit()
        {
            base.Exit();
            owner.StopAttack();
        }
    }
}

