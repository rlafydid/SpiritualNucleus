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
        public static Func<int, Vector3, HeroActorController> CreateHero;

        public static Func<long, SceneActorController> GetActor;

        public static Func<List<SceneActorController>> GetMonsters;
        public static Func<List<SceneActorController>> GetHeroActors;

        public static Func<SceneActorController, List<SceneActorController>, float, SceneActorController> SelectNearestTarget;

        public static Action<long> ReleaseActor;
    }
}

namespace Battle
{
    public class ActorModule : BaseBattleModule
    {
        List<SceneActorController> heroActors = new List<SceneActorController>();
        List<SceneActorController> monsterActors = new List<SceneActorController>();

        public override void Init()
        {
            Facade.Battle.CreateHero = CreateHero;
            Facade.Battle.StartBattle = StartBattle;
            Facade.Battle.GetActor = GetActor;
            Facade.Battle.GetMonsters = GetActors;
            Facade.Battle.GetHeroActors = GetHeroActors;
            Facade.Battle.SelectNearestTarget = SelectNearestTarget;
            Facade.Battle.ReleaseActor = ReleaseActor;
        }

        public HeroActorController CreateHero(int heroId, Vector3 pos = default)
        {
            HeroActorController actor = new HeroActorController();
            actor.SetConfig(heroId);
            actor.OnInit();
            actor.Load();
            actor.Position = pos.ToGroundPos();
            heroActors.Add(actor);
            return actor;
        }

        public SceneActorController CreateMonster(int monsterID, Vector3 pos = default)
        {
            MonsterActorController actor = new MonsterActorController();
            actor.SetConfig(monsterID);
            actor.OnInit();
            actor.Load();
            actor.Position = pos.ToGroundPos();
            monsterActors.Add(actor);
            return actor;
        }

        public override void StartBattle()
        {
            Debug.Log("Start");
            var ctrl = SceneManager.Instance.Camera.AddComponent<CameraMoveController>();

            Vector3 heroPos = new Vector3(-7.86f, 3.61f, -12.77f);
            CreateHero(1003, heroPos);

            Vector3 heroForward = heroPos + Vector3.forward * 10;

            // float range = 20;
            // float farRange = 100;
            // for (int i = 0; i < 10; i++)
            // {
            //     float x = UnityEngine.Random.Range(heroForward.x - range, heroForward.x + range);
            //     float y = UnityEngine.Random.Range(heroForward.z, heroForward.z + farRange);
            //     CreateMonster(2002, new Vector3(x, 0, y));
            // }
            // ctrl.TraceTarget(heroActors[0].Entity);
        }

        public override void Update()
        {
            base.Update();
            foreach (var act in heroActors)
            {
                act.Update();
            }
            foreach (var act in monsterActors)
            {
                act.Update();
            }
        }

        public override void LateUpdate()
        {
            base.LateUpdate();
            foreach (var act in heroActors)
            {
                act.LateUpdate();
            }
            foreach (var act in monsterActors)
            {
                act.LateUpdate();
            }
        }

        void ReleaseActor(long id)
        {
            var act = heroActors.Find(d => d.ID == id);
            if (act != null)
            {
                heroActors.Remove(act);
            }
            else
            {
                act = monsterActors.Find(d => d.ID == id);
                monsterActors.Remove(act);
            }
            act.Destroy();
        }


        SceneActorController GetActor(long id)
        {
            var act = heroActors.Find(d => d.ID == id);
            if (act == null)
                act = monsterActors.Find(d => d.ID == id);
            return act;
        }

        List<SceneActorController> GetActors()
        {
            return monsterActors;
        }

        List<SceneActorController> GetHeroActors()
        {
            return heroActors;
        }

        SceneActorController SelectNearestTarget(SceneActorController owner, List<SceneActorController> actors, float range)
        {
            SceneActorController selectActor = null;
            float lastDistance = int.MaxValue;
            foreach (var item in actors)
            {
                float distance = Vector3.Distance(owner.Position, item.Position);
                if (distance < range && distance < lastDistance)
                {
                    lastDistance = distance;
                    selectActor = item;
                }
            }

            return selectActor;
        }
    }
}
