using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using LKEngine;
using UnityEngine;
using Random = UnityEngine.Random;

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
            actor.SetConfig((int)heroId);
            actor.OnInit();
            actor.Load();
            actor.Position = pos.ToGroundPos();
            heroActors.Add(actor);
            return actor;
        }

        public SceneActorController CreateMonster(long monsterID, Vector3 pos = default)
        {
            MonsterActorController actor = new MonsterActorController();
            actor.SetConfig((int)monsterID);
            actor.OnInit();
            actor.Load();
            actor.Position = pos.ToGroundPos();
            monsterActors.Add(actor);
            return actor;
        }

        public override void StartBattle()
        {
            Debug.Log("Start");

            var sceneConfig = Facade.Battle.GetConfig();

            foreach (var pointConfig in sceneConfig.pointConfigs)
            {
                switch (pointConfig.generateType)
                {
                    case EGenerateType.Hero:
                        CreateHero((int)pointConfig.id, pointConfig.point);
                        break;
                    case EGenerateType.Monster:
                        for (int i = 0; i < pointConfig.count; i++)
                        {
                            float randomRadius = pointConfig.radius;
                            float randomAngle = Random.Range(0, 360);
                            float cosVal = Mathf.Cos(randomAngle);
                            float sinVal = Mathf.Sin(randomAngle);

                            float x = sinVal * randomRadius;
                            float y = cosVal * randomRadius;
                            CreateMonster(pointConfig.id, pointConfig.point + new Vector3(x,0,y));
                        }
                        break;
                }
            }
            
            var ctrl = SceneManager.Instance.Camera.AddComponent<CameraMoveController>();
            ctrl.TraceTarget(heroActors[0].Entity);
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

            if (_releasedActors.Count > 0)
            {
                foreach (var id in _releasedActors)
                {
                    RemoveActor(id);
                }
                _releasedActors.Clear();
            }
            
            foreach (var act in heroActors)
            {
                act.LateUpdate();
            }
            foreach (var act in monsterActors)
            {
                act.LateUpdate();
            }
        }

        private List<long> _releasedActors = new List<long>();
        void ReleaseActor(long id)
        {
            _releasedActors.Add(id);
        }

        void RemoveActor(long id)
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
