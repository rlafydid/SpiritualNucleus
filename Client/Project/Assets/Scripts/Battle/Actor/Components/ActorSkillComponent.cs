using System.Collections;
using System.Collections.Generic;
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

        protected override void OnAwake()
        {
            base.OnAwake();
            normalAttacksComp = this.AddComponent<NormalAttacksComponent>();
            normalAttacksComp.Setup(ownerActor);
        }

        protected override void OnStart()
        {
            base.OnStart();

            skillMapping = new Dictionary<int, SkillUnit>();

            var conf = ownerActor.CharacterConfig;

            skillMapping.Add(1, new SkillUnit() { BPRes = conf.Skill1, OwnerID = this.ownerActor.ID });
            skillMapping.Add(2, new SkillUnit() { BPRes = conf.Skill2, OwnerID = this.ownerActor.ID });
            skillMapping.Add(3, new SkillUnit() { BPRes = conf.Skill3, OwnerID = this.ownerActor.ID });
        }
    
        public bool FindSkill()
        {
            foreach(var item in skillMapping)
            {
                if(Input.GetKeyDown(item.Key))
                {
                    readySkill = item.Value;
                    return true;
                }
            }
            if(Input.GetKeyDown(KeyCode.J))
            {
                readySkill = normalAttacksComp.FindSkill();
                return true;
            }
            readySkill = null;
            return false;
        }
        
        public void SetupSkill(int index)
        {
            var abilityController = this.ownerActor.Entity.GameObject.GetComponent<AbilityController>();
            abilityController.UseAbility(index);
        }
    }
}
