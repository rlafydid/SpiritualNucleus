using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using UnityEngine;

namespace Battle
{
    public class SkillComponent : ActorComponent
    {
        protected SkillUnit readySkill;

        public virtual SkillUnit GetReadySkill()
        {
            return readySkill;
        }
    }

    public class HeroSkillComponent : SkillComponent
    {
        Dictionary<int, SkillUnit> skillMapping;
        NormalAttacksComponent normalAttacksComp;

        private int _normalAttackIndex;

        private AbilityController _abilityController;
        
        protected override void OnAwake()
        {
            base.OnAwake();
           
        }

        protected override void OnStart()
        {
            base.OnStart();
            _abilityController = this.ownerActor.Entity.GameObject.GetComponent<AbilityController>();
            normalAttacksComp = this.AddComponent<NormalAttacksComponent>();
            normalAttacksComp.Setup(ownerActor);
            skillMapping = new Dictionary<int, SkillUnit>();

            var conf = ownerActor.CharacterConfig;

            skillMapping.Add(1, new SkillUnit() { BPRes = conf.Skill1, OwnerID = this.ownerActor.ID });
            skillMapping.Add(2, new SkillUnit() { BPRes = conf.Skill2, OwnerID = this.ownerActor.ID });
            skillMapping.Add(3, new SkillUnit() { BPRes = conf.Skill3, OwnerID = this.ownerActor.ID });

            this.ownerActor.Entity.GameObject.GetComponent<AbilitySystemCharacter>().OwnerId = this.ownerActor.ID;
        }
        //
        // public bool FindSkill()
        // {
        //     foreach(var item in skillMapping)
        //     {
        //         if(Input.GetKeyDown(item.Key))
        //         {
        //             readySkill = item.Value;
        //             return true;
        //         }
        //     }
        //     if(Input.GetKeyDown(KeyCode.J))
        //     {
        //         readySkill = normalAttacksComp.FindSkill();
        //         return true;
        //     }
        //     readySkill = null;
        //     return false;
        // }
        //
        public async void UseNormalAbility()
        {
            _normalAttackIndex = ++_normalAttackIndex % 4;
            await _abilityController.UseNormalAbility(_normalAttackIndex);
            this.ownerActor.GetComponent<FSM.FiniteStateMachine>().TriggerEvent(ERoleState.Idle);
        }
        
        public async void UseAbility(int index)
        {
            await _abilityController.UseAbility(index);
            this.ownerActor.GetComponent<FSM.FiniteStateMachine>().TriggerEvent(ERoleState.Idle);
        }
    }
}
