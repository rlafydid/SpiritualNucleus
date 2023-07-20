using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act
{
    /// <summary>
    /// 位于空间
    /// </summary>
    public enum ESpace
    {
        World,
        Local
    }

    /// <summary>
    /// 生成位置
    /// </summary>
    public enum EGeneratePoint
    {
        Owner, //拥有者位置
        Target, //目标位置
    }

    /// <summary>
    /// 行为
    /// </summary>
    public enum EActionMode
    {
        None, //无行为
        FollowRoot, //跟随模型
        FollowNode, //跟随点
    }

    /// <summary>
    /// 跟随类型
    /// </summary>
    public enum EFollowType
    {
        Position,
        PositionAndRotation,
    }

    /// <summary>
    /// 朝向
    /// </summary>
    public enum EDirection
    {
        OwnerForward,
        TurnToTarget,
    }

    [ActDisplayName("特效", "播放特效"), Serializable]
    public class ActEffectClip : ActBaseClip
    {
        //public string assetName;
        public GameObject assetPrefab;

        public EGeneratePoint generateMode = EGeneratePoint.Owner;
        public EDirection direction = EDirection.OwnerForward;
        public ESpace space = ESpace.World;
        public EActionMode actionMode = EActionMode.None;
        public EFollowType followType = EFollowType.Position;

        public string bindPoint = "Default";

        public float speed = 1;

        public Vector3 worldOffset;
        public Vector3 localOffset;

        public float scale = 1;
        public Vector3 eulerAngles = Vector3.zero;

        public bool isLoop = false;
        public bool isTurnToTarget;
        public bool dontDestroyWithAct;

        public override bool SupportResize => isLoop;

        #region 不序列化的字段
        GameObjectEntity getOwner { get => ActInstance.Owner as GameObjectEntity; }

        ActEntity getTarget
        {
            get
            {
                if (ActInstance.TargetList != null && ActInstance.TargetList.Count > 0)
                {
                    return ActInstance.TargetList[0];
                }
                return null;
            }
        }

        //string getAssetName
        //{
        //    get
        //    {
        //        if (Facade.Externals.GetResourceName != null)
        //            return Facade.Externals.GetResourceName(assetName, 1);
        //        else
        //            return assetName;
        //    }
        //}

        class ActEffectInstance
        {
            public Vector3 Offset;
            public ActEntity Effect;
            public ActEntity Target;

            public class InitialData
            {
                public Vector3 Pos;
                public Quaternion Rotation;
            }

            public InitialData initialData;
        }


        [NonSerialized]
        List<ActEffectInstance> effectInstances = new List<ActEffectInstance>();
        #endregion


        public override void Preload()
        {
            base.Preload();
        }

        public override bool OnTrigger()
        {
            if (assetPrefab == null)
            {
                Debug.LogError($"{ActInstance.ActName}中特效为空, 请配置特效.");
                return false;
            }
            switch (generateMode)
            {
                case EGeneratePoint.Owner:
                    LoadOwner();
                    break;
                case EGeneratePoint.Target:
                    LoadTargets();
                    break;
            }
            return true;
        }

        void LoadOwner()
        {
            ActEntity effect = ActEntityFactory.Instantiate(assetPrefab);

            ActEffectInstance instance = new ActEffectInstance()
            {
                Effect = effect,
                Target = getOwner
            };
            effectInstances.Add(instance);

            AddEffect(effect, getOwner, instance);
            SetDirection(effect, getOwner, getTarget);
        }

        void LoadTargets()
        {
            if (ActInstance.TargetList == null)
                return;

            for (int i = 0; i < ActInstance.TargetList.Count; i++)
            {
                var target = ActInstance.TargetList[i];
                //ActEntity effect = ActEntityFactory.CreateEntity(getAssetName);
                ActEntity effect = ActEntityFactory.Instantiate(assetPrefab);

                ActEffectInstance instance = new ActEffectInstance()
                {
                    Effect = effect,
                    Target = target,
                };
                effectInstances.Add(instance);

                AddEffect(effect, target, instance);
                SetDirection(effect, getOwner, target);
            }
        }

        void AddEffect(ActEntity effectEntity, ActEntity target, ActEffectInstance instance)
        {
            if (isDestroted)
            {
                effectEntity.Destroy();
                return;
            }

            SetData(effectEntity, target, instance);

            ParticleComponnet particleComp = effectEntity.AddComponent<ParticleComponnet>();
            particleComp.SetLoop(isLoop);
            if (isDebugMode)
                particleComp.OpenDebug();
            else
            {
                particleComp.Play();
                if (dontDestroyWithAct)
                {
                    Facade.Externals.Delay(Duration, effectEntity.Destroy);
                }
            }
            particleComp.SetSpeed(speed * ActInstance.PlaySpeed);
        }


        void SetData(ActEntity effectEntity, ActEntity target, ActEffectInstance instance)
        {
            GameObject effect = (effectEntity as GameObjectEntity).GameObject;

            Vector3 offset = worldOffset;

            GameObject bindPointObj = target.GetAttachedPoint(bindPoint);

            switch (space)
            {
                case ESpace.World:
                    effect.transform.parent = null;

                    offset += target.Rotation * localOffset;
                    effect.transform.position = bindPointObj.transform.position + offset;

                    break;
                case ESpace.Local:
                    offset += localOffset;

                    effect.transform.parent = bindPointObj.transform;
                    effect.transform.localPosition = offset;
                    break;
            }

            instance.initialData = new ActEffectInstance.InitialData()
            {
                Pos = bindPointObj.transform.position,
                Rotation = target.Rotation
            };

            effect.transform.localRotation = Quaternion.Euler(eulerAngles);

            effect.transform.localScale = Vector3.one * scale;
        }

        void SetDirection(ActEntity effect, ActEntity owner, ActEntity target)
        {
            if (space == ESpace.Local)
            {
                return;
            }
            switch (direction)
            {
                case EDirection.OwnerForward:
                    effect.Rotation = owner.Rotation * Quaternion.Euler(eulerAngles);
                    break;
                case EDirection.TurnToTarget:
                    if (target != null && target.IsValid)
                    {
                        effect.Rotation = Quaternion.LookRotation((target.Position - owner.Position).normalized, Vector3.up) * Quaternion.Euler(eulerAngles);
                    }
                    break;
            }
        }

        void UpdateAction()
        {
            if (space == ESpace.Local || actionMode == EActionMode.None)
                return;

            for (int i = 0; i < effectInstances.Count; i++)
            {
                var instance = effectInstances[i];
                if (!instance.Effect.IsValid || !instance.Target.IsActive)
                    continue;

                Vector3 offset = Vector3.zero;

                GameObject followTarget = null;
                switch (actionMode)
                {
                    case EActionMode.FollowRoot:
                        followTarget = (instance.Target as GameObjectEntity).GameObject;

                        if (followType == EFollowType.Position)
                            instance.Effect.Position = instance.Target.Position + localOffset;
                        else
                            instance.Effect.Position = instance.Target.Position + instance.Target.Rotation * localOffset;
                        break;
                    case EActionMode.FollowNode:
                        followTarget = instance.Target.GetAttachedPoint(bindPoint);

                        if (followType == EFollowType.Position)
                            instance.Effect.Position = followTarget.transform.position + localOffset;
                        else
                            instance.Effect.Position = followTarget.transform.position + instance.Target.Rotation * localOffset;
                        break;
                }
            }

        }

        public void RefreshByInitialData()
        {
            switch (space)
            {
                case ESpace.World:
                    if (actionMode == EActionMode.None)
                    {
                        for (int i = 0; i < effectInstances.Count; i++)
                        {
                            var instance = effectInstances[i];
                            if (!instance.Effect.IsValid || !instance.Target.IsActive)
                                continue;

                            GameObject effect = (instance.Effect as GameObjectEntity).GameObject;
                            effect.transform.position = instance.initialData.Pos + instance.initialData.Rotation * localOffset;

                            effect.transform.localRotation = Quaternion.Euler(eulerAngles);

                            effect.transform.localScale = Vector3.one * scale;
                        }
                    }
                    break;
                case ESpace.Local:
                    for (int i = 0; i < effectInstances.Count; i++)
                    {
                        var instance = effectInstances[i];
                        if (!instance.Effect.IsValid || !instance.Target.IsActive)
                            continue;

                        GameObject effect = (instance.Effect as GameObjectEntity).GameObject;

                        effect.transform.localRotation = Quaternion.Euler(eulerAngles);

                        effect.transform.localScale = Vector3.one * scale;

                        effect.transform.localPosition = localOffset;
                    }
                    break;
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            if (effectInstances.Count > 0)
            {
                for (int i = 0; i < effectInstances.Count; i++)
                {
                    effectInstances[i].Effect.SetActive(true);
                }
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            for (int i = 0; i < effectInstances.Count; i++)
            {
                effectInstances[i].Effect.SetActive(false);
            }
        }

        public override void Destory()
        {
            base.Destory();
            Release();
        }

        //重新出发
        public void ReTrigger()
        {
            Release();
            OnTrigger();
        }

        void Release()
        {
            for (int i = 0; i < effectInstances.Count; i++)
            {
                if (isDebugMode || !dontDestroyWithAct)
                    effectInstances[i].Effect.Destroy();
            }
            effectInstances.Clear();
        }

        float timer;
        public override void Update(float time, float deltaTime)
        {
            this.timer = time;
            UpdateAction();
            if (isDebugMode)
            {
                for (int i = 0; i < effectInstances.Count; i++)
                {
                    var instance = effectInstances[i];
                    if (!instance.Effect.IsValid)
                        continue;

                    instance.Effect.GetComponent<ParticleComponnet>().Simulate(timer);
                }
            }
        }

        public override void ChangeSpeed(float speed)
        {
            base.ChangeSpeed(speed);
            for (int i = 0; i < effectInstances.Count; i++)
            {
                var instance = effectInstances[i];
                if (!instance.Effect.IsValid)
                    continue;

                instance.Effect.GetComponent<ParticleComponnet>().SetSpeed(speed * ActInstance.PlaySpeed);
            }
        }
    }
}

