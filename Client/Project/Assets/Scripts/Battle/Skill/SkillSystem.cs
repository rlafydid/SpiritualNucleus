using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using GraphProcessor;
using UnityEngine;

namespace Facade
{
    public class Skill
    {
        public static Action<SkillUnit> TriggerSkill;
        public static Action<long> StopSkill;

        public static Action<SkillUnit> TriggerLauncher;
    }
}

namespace Battle
{
    
    public class SkillMudule : BaseBattleModule
    {
        public List<SkillProcess> skillList = new List<SkillProcess>();

        Dictionary<long, SkillProcess> skillMapping = new Dictionary<long, SkillProcess>();

        public override void Init()
        {
            base.Init();
            Facade.Skill.TriggerSkill = TriggerSkill;
            Facade.Skill.StopSkill = Stop;
        }

        public override void Update()
        {
            base.Update();
            if (skillList == null)
                return;

            for(int i = skillList.Count-1; i >= 0; i--)
            {
                var process = skillList[i];
                if(process.IsFinished)
                    skillList.RemoveAt(i);
                else
                    process.Update();
            }
            //foreach (var process in skillList)
            //{
            //    process.Update();
            //}
        }

        void TriggerSkill(SkillUnit skill)
        {
            SkillProcess process = new SkillProcess();
            process.Init(skill);
            process.Load();
            process.Finish = Finish;
            skillList.Add(process);
        }

        void Stop(long id)
        {
            var skill = skillList.Find(d => d.OwnerId == id);
            if(skill != null)
            {
                skill.IsFinished = true;
            }
        }

        void Finish(SkillProcess process)
        {
            process.IsFinished = true;
        }

    }

    public class LauncherMudule : BaseBattleModule
    {
        public List<SkillProcess> skillList = new List<SkillProcess>();

        public override void Init()
        {
            base.Init();
            Facade.Skill.TriggerLauncher = TriggerLauncher;
        }

        void TriggerLauncher(SkillUnit unit)
        {
            SkillProcess process = new SkillProcess();
            process.Init(unit);
            process.Load();
            process.Finish = Finish;
            skillList.Add(process);
        }

        public override void Update()
        {
            base.Update();

            for (int i = skillList.Count - 1; i >= 0; i--)
            {
                var process = skillList[i];
                if (process.IsFinished)
                    skillList.RemoveAt(i);
                else
                    process.Update();
            }
        }

        void Finish(SkillProcess process)
        {
            process.IsFinished = true;
        }
    }
}
