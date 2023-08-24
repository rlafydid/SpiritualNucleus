using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using UnityEngine;

namespace Battle
{
    public struct AttackStateData : IStateData
    {
        public long abilityId;
    }
    public class HeroAttackState : HeroState<AttackStateData>
    {
        SkillUnit triggerSkill;
        SceneActorController target;
        float maxDistance = 3f;

        bool isContinue = false;

        public override async void Enter()
        {
            owner.StopMove();
            if (target == null || target.IsDead() || Vector3.Distance(owner.Position, target.Position) > maxDistance)
            {
                var monsters = Facade.Battle.GetMonsters();
                target = Facade.Battle.SelectNearestTarget(owner, monsters, maxDistance);
            }

            if (target != null)
            {
                Vector3 lookAtPos = target.Position;
                lookAtPos.y = owner.Position.y;
                //owner.Entity.LookAt(lookAtPos);
            }

            var ability = owner.GetComponent<HeroSkillComponent>().GetAbility(Data.abilityId);
            await ability.TryActivateAbility();
            this.ChangeState(ERoleState.Idle);

            // triggerSkill = owner.GetComponent<HeroSkillComponent>().GetReadySkill();
            // triggerSkill.Finish = Finish;
            // Facade.Skill.TriggerSkill(triggerSkill);
            
            // owner.GetComponent<AbilityController>()

        }


        public override void Update()
        {
            //if (Input.GetKeyDown(KeyCode.J))
            //{
            //    isContinue = true;
            //}
        }

        void Finish(SkillUnit unit)
        {
            //if(isContinue)
            //{
            //    owner.GetComponent<HeroSkillComponent>().FindSkill();
            //    Enter();
            //}
            //else
                this.ChangeState(ERoleState.Idle);
        }
    }

}
