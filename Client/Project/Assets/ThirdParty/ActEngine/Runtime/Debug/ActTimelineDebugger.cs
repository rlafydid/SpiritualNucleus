using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Act
{
    public class ActTimelineDebugger
    {
        static ActInstance actInstance;
        public static bool previewMode;

        public static void CreateDebugger(ActAsset asset)
        {
            if (actInstance != null)
                actInstance.Destroy();


            GameObject owner = Facade.Preview.GetMainCharactor();
            actInstance = new ActInstance(asset);
            actInstance.Reset();
            actInstance.Start(owner.ToActEntity());
            actInstance.TargetList = new List<ActEntity>() { Facade.Preview.GetTarget().ToActEntity() };
            actInstance.OpenDebugMode();

            Facade.Internals.Timeline.FixedUpdate = FixedUpdate;
            ActGizmosViewer.Instance.Add(DrawBulletEvent);
        }

        public static void RefreshData()
        {
            actInstance?.Refresh();
        }

        static float simulateTime;
        public static void Simulate(float time)
        {
            if (actInstance == null)
                return;

            simulateTime = time;
            actInstance?.UpdateByTime(simulateTime);
            CheckTriggerEvents(time);
        }

        static float timer;
        static bool isPlaying;
        public static void Play()
        {
            if (Application.isPlaying)
                isPlaying = true;
        }

        public static void Pause()
        {
            isPlaying = false;
        }

        public static void Clear()
        {
            actInstance?.Clear();
        }

        public static void Destroy()
        {
            actInstance?.Destroy();
            Clear();
            actInstance = null;
            Facade.Internals.Timeline.FixedUpdate = null;
            ActGizmosViewer.Instance.Clear();
        }

        static void FixedUpdate()
        {
            if (Application.isPlaying && previewMode)
                actInstance?.UpdateByTime(simulateTime);

            if (isPlaying)
            {
                Act.Facade.Internals.Timeline.AddFrame?.Invoke();
            }

        }

        static void DrawBulletEvent()
        {
            if (actInstance != null && actInstance.GetAsset.Events != null)
            {
                GameObject owner = Facade.Preview.GetMainCharactor();

                foreach (var item in actInstance.GetAsset.Events)
                {
                    if (item is ActBulletEvent bulletEvent && bulletEvent.BulletData != null)
                    {
                        var offsetPos = owner.transform.position + owner.transform.rotation * bulletEvent.BulletData.launchOffset;
                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(offsetPos, 0.1f);
                        Gizmos.DrawLine(offsetPos, offsetPos + owner.transform.forward * 2);
                    }
                }
            }
        }

        static void CheckTriggerEvents(float time)
        {
            if (actInstance != null && actInstance.GetAsset.Events != null)
            {
                GameObject owner = Facade.Preview.GetMainCharactor();

                foreach (var item in actInstance.GetAsset.Events)
                {
                    if (time >= item.TriggerTime && time < item.TriggerTime + Time.fixedDeltaTime - 0.01f)
                    {
                        Facade.Preview.TriggerEvent(item);
                    }
                }
            }

        }
    }
}

