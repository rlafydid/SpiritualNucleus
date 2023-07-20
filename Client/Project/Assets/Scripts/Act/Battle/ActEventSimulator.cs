using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act.Simulator
{
    class ActEventSimulator : ActSimulator
    {
        public override bool canSimulate => true;

        List<ActBaseEvent> events = new List<ActBaseEvent>();

        public override void Start()
        {
            base.Start();
            Facade.Preview.TriggerEvent = TriggerEvent;
        }

        bool canUpdate;
        public override void StartPlayingAct(ActInstance actInstance)
        {
            base.StartPlayingAct(actInstance);
            events.AddRange(actInstance.GetAsset.Events);
            timer = 0;
            canUpdate = true;
        }

        public override void Update()
        {
            if (!canUpdate)
                return;

            UpdateByTime(this.actTimer);
        }

        public override void UpdateByTime(float time)
        {
            for (int i = events.Count - 1; i >= 0; i--)
            {
                if (time >= events[i].TriggerTime)
                {
                    TriggerEvent(events[i]);
                    events.RemoveAt(i);
                }
            }
            if (events.Count == 0)
                canUpdate = false;
        }

        void TriggerEvent(ActBaseEvent actEvent)
        {
            switch (actEvent)
            {
                case ActDamageEvent damageEvent:
                    TriggerDamageEvent(damageEvent);
                    break;
                case ActBulletEvent bulletEvent:
                    owner.GetSimulator<ActBulletSimulator>().TriggerBulletEvent(bulletEvent);
                    break;
            }
        }

        void TriggerDamageEvent(ActDamageEvent actEvent)
        {
            ActorController mainCharactor = BattleFacade.GetMainCharactor();
            foreach (var item in BattleFacade.GetTargets())
            {
                if (actEvent.AttackRange == 0 || Vector3.Distance(item.Position, mainCharactor.Position) <= actEvent.AttackRange)
                    item.Hit();
            }
        }

        public override void Destroy()
        {
            base.Destroy();
            events.Clear();
        }
    }
}

