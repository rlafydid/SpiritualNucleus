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
        public static Action<long> EnterLevel;
        public static Func<SceneConfig> GetConfig;
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

        private ChapterConfig _chapterConfig;
        private SceneConfig _sceneConfig;

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
            Facade.Battle.EnterLevel = EnterLevel;
            Facade.Battle.GetConfig = GetConfig;
            _chapterConfig = Facade.Asset.Load<ChapterConfig>("ChapterConfig");
        }

        SceneConfig GetConfig()
        {
            return _sceneConfig;
        }

        void EnterLevel(long id)
        {
            _sceneConfig = _chapterConfig.configs.Find(d => d.id == id);
            SceneManager.Instance.LoadScene(SceneType.Battle, _sceneConfig.sceneName);
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
