using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class HeroAttackState : HeroState
    {
        SkillUnit triggerSkill;
        SceneActorController target;
        float maxDistance = 3f;

        bool isContinue = false;

        public override void Enter()
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
            triggerSkill = owner.GetComponent<HeroSkillComponent>().GetReadySkill();
            triggerSkill.Finish = Finish;
            Facade.Skill.TriggerSkill(triggerSkill);

            isContinue = false;
        }


        public override void Exexute()
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
