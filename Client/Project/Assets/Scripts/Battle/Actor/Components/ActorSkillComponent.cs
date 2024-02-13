using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using FSM;
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
            skillMapping = new Dictionary<int, SkillUnit>();

            var conf = ownerActor.CharacterConfig;

            skillMapping.Add(1, new SkillUnit() { BPRes = conf.Skill1, OwnerID = this.ownerActor.ID });
            skillMapping.Add(2, new SkillUnit() { BPRes = conf.Skill2, OwnerID = this.ownerActor.ID });
            skillMapping.Add(3, new SkillUnit() { BPRes = conf.Skill3, OwnerID = this.ownerActor.ID });
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
        public void UseNormalAbility()
        {
            _normalAttackIndex = _normalAttackIndex % _abilityController.NormalAbilities.Length;
            var ability = _abilityController.GetNormalAbility(_normalAttackIndex);
            if (TryUseAbility(ability))
            {
                _normalAttackIndex++;
            }
        }
        
        public async void UseAbility(int index)
        {
            var ability = _abilityController.GetAbility(index);
            TryUseAbility(ability);
        }

        bool TryUseAbility(AbstractAbilitySpec ability)
        {
            if (ability.CanActivateAbility())
            {
                var abilitySpec = ability as UniversalAbilityScriptableObject.UniversalAbilitySpec;
                var attackStateData = new AttackStateData() { abilityId = abilitySpec.AbilityId };
                return this.ownerActor.ChangeState(ERoleState.Attack,  attackStateData);
            }

            return false;
        }

        public AbstractAbilitySpec GetAbility(long id)
        {
            return _abilityController.GetAbilityById(id);
        }
    }
}
