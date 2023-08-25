using System;
using UnityEngine;

namespace Battle
{
    public class MonsterAttackState : MonsterState<AttackStateData>
    {
        SkillUnit triggerSkill;
        SceneActorController target;
        float maxDistance = 3f;

        public override async void Enter()
        {
            owner.StopMove();
            // if (target == null || Vector3.Distance(owner.Position, target.Position) > maxDistance)
            // {
            //     var monsters = Facade.Battle.GetMonsters();
            //     target = Facade.Battle.SelectNearestTarget(owner, monsters, maxDistance);
            // }
            //
            // if (target != null)
            // {
            //     Vector3 lookAtPos = target.Position;
            //     lookAtPos.y = owner.Position.y;
            //     owner.Entity.LookAt(lookAtPos);
            // }

            var ability = owner.GetComponent<HeroSkillComponent>().GetAbility(Data.abilityId);
            await ability.TryActivateAbility();
            this.ChangeState(ERoleState.Idle);
            
            // Facade.Skill.TriggerSkill(triggerSkill);
        }


        public override void Update()
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

