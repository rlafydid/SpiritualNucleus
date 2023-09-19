using System;
using UnityEngine;

namespace Battle
{
    public class MonsterAttackState : MonsterState<AttackStateData>
    {
        SkillUnit triggerSkill;
        SceneActorController target;
        float maxDistance = 3f;

        protected override async void OnEnter()
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
            this.ToDefaultState();
            
            // Facade.Skill.TriggerSkill(triggerSkill);
        }


        protected override void OnUpdate()
        {

        }

        void Finish(SkillUnit unit)
        {
            this.ToDefaultState();
        }

        protected override void OnExit()
        {
            owner.StopAttack();
        }
    }
}

