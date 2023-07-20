using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using LKEngine;
using UnityEngine;

namespace Facade
{
    public partial class Battle
    {
        public static Action StartBattle;
    }
}

namespace Battle
{
    public class BaseBattleModule : BaseModule
    {
        public virtual void StartBattle() { }
    }

    public class BattleModule : BaseModule
    {
        List<BaseBattleModule> battleModules = new List<BaseBattleModule>();
        public override void Init()
        {
            base.Init();
            battleModules.Add(new ActorModule());
            battleModules.Add(new CommandModule());
            battleModules.Add(new SkillMudule());
            battleModules.Add(new LauncherMudule());
            battleModules.Add(new BulletModule());

            foreach (var item in battleModules)
            {
                item.Init();
            }
            Facade.Battle.StartBattle = StartBattle;
        }

        public override void Update()
        {
            base.Update();
            foreach (var item in battleModules)
            {
                item.Update();
            }
        }

        public override void LateUpdate()
        {
            foreach (var item in battleModules)
            {
                item.LateUpdate();
            }
        }

        void StartBattle()
        {
            foreach (var item in battleModules)
            {
                item.StartBattle();
            }
        }
    }
}
