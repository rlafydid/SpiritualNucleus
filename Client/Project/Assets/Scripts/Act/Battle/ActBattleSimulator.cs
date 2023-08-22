using Battle;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act.Simulator
{
    class BattleFacade
    {
        internal static Func<ActorController> GetMainCharactor;
        internal static Func<List<ActorController>> GetTargets;
    }

    class ConstParams
    {
        public static float moveSpeed = 1;
        public static float moveRange = 1.5f;
    }

    public class ActBattleSimulator : MonoBehaviour
    {
        List<ActSimulator> simulators = new List<ActSimulator>();

        ActInstance actInstance;

        ActorController mainCharactor;
        List<ActorController> targets = new List<ActorController>();

        string lastPlayedAct = null;

        //bool isDebugging

        void Start()
        {
            Facade.Preview.PlayAct = PlayAct;
            Facade.Preview.StopAct = StopAct;
            Facade.Preview.ReloadPreview = StartSimulate;

            BattleFacade.GetTargets = GetTargets;
            BattleFacade.GetMainCharactor = GetMainCharactor;

            Facade.Internals.Timeline.OpenDebugMode = OpenTimelineDebugMode;
            Facade.Internals.Timeline.Simulate = Simulate;

            StartSimulate();
        }

        public void StartSimulate()
        {
            OnDestroy();

            AddSimulator<ActEventSimulator>();
            AddSimulator<ActBulletSimulator>();
            // AddSimulator<ActMoveSimulator>();

            mainCharactor = new ActorController();
            mainCharactor.Entity = Facade.Preview.GetMainCharactor();

            targets.Clear();

            GameObject target = Facade.Preview.GetTarget();

            if (target != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    var obj = GameObject.Instantiate(target);
                    obj.transform.localScale = Vector3.one;
                    obj.transform.LookAt(mainCharactor.Position);
                    obj.transform.position = target.transform.position + Vector3.right * (i + 1) * 1.5f;
                    var actor = new ActorController();
                    actor.Entity = obj;
                    actor.StartMove();
                    targets.Add(actor);
                }
                GameObject.Destroy(target);
            }
        }

        public void PlayAct(string path, float speed = 1)
        {
            if (string.IsNullOrEmpty(path))
                return;

            actInstance = mainCharactor.PlayAct(path);
            actInstance.PlaySpeed = speed;
            actInstance.LoadComplete = LoadActComplete;
            lastPlayedAct = path;
        }

        public void StopAct()
        {
            foreach (var simulator in simulators)
            {
                simulator.Destroy();
            }

            mainCharactor.StopAct();
            actInstance = null;
        }

        void LoadActComplete()
        {
            foreach (var simulator in simulators)
            {
                simulator.StartPlayingAct(actInstance);
            }
        }

        // Update is called once per frame
        public void Update()
        {
            foreach (var simulator in simulators)
            {
                simulator.timer += Time.deltaTime;
                simulator.Update();
            }

            mainCharactor?.Update();
            foreach (var item in targets)
            {
                item.Update();
            }

            // if (Input.GetKeyDown(KeyCode.Space))
            // {
            //     PlayAct(lastPlayedAct);
            // }
        }

        internal ActorController GetMainCharactor()
        {
            return mainCharactor;
        }

        internal List<ActorController> GetTargets()
        {
            return targets;
        }

        void AddSimulator<T>() where T : ActSimulator, new()
        {
            T t = new T();
            t.owner = this;
            t.Start();
            simulators.Add(t);
        }

        internal T GetSimulator<T>() where T : ActSimulator
        {
            return (T)simulators.Find(d => d.GetType() == typeof(T));
        }

        private void OnDestroy()
        {
            foreach (var simulator in simulators)
            {
                simulator.Destroy();
            }
            mainCharactor?.Destroy();
            foreach (var item in targets)
            {
                item.Destroy();
            }
            mainCharactor = null;
            targets.Clear();

            simulators.Clear();
        }

        void OpenTimelineDebugMode()
        {
            actInstance.OpenDebugMode();
        }

        void Simulate(float time)
        {
            foreach (var simulator in simulators)
            {
                simulator.UpdateByTime(time);
            }
        }

        public void OnGUI()
        {
            foreach (var simulator in simulators)
            {
                simulator.OnGUI();
            }
        }

        private void OnDrawGizmos()
        {
            foreach (var simulator in simulators)
            {
                simulator.OnDrawGizmos();
            }
        }
    }

}

