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
        Dictionary<KeyCode, SkillUnit> skillMapping;
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

            skillMapping = new Dictionary<KeyCode, SkillUnit>();

            var conf = ownerActor.CharacterConfig;

            skillMapping.Add(KeyCode.K, new SkillUnit() { BPRes = conf.Skill1, OwnerID = this.ownerActor.ID });
            skillMapping.Add(KeyCode.U, new SkillUnit() { BPRes = conf.Skill2, OwnerID = this.ownerActor.ID });
            skillMapping.Add(KeyCode.I, new SkillUnit() { BPRes = conf.Skill3, OwnerID = this.ownerActor.ID });
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
    }
}
