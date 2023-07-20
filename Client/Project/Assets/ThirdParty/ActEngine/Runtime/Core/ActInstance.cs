using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act
{
    public class ActInstance
    {
        enum ClipState
        {
            WaitTrigger, //等待触发
            Triggered, //已触发
            Disabled //已失效
        }

        ActAsset data;

        float _playSpeed = 1;
        public float PlaySpeed
        {
            get => _playSpeed;
            set
            {
                _playSpeed = value;
                if (isLoaded)
                {
                    for (int i = 0; i < data.Clips.Count; i++)
                    {
                        if (clipState[i] == ClipState.Triggered)
                        {
                            data.Clips[i].ChangeSpeed(value);
                        }
                    }
                }
            }
        }

        public bool LifeOver { get; set; }

        public Vector3 TargetPosition { get; set; }

        string actName;
        public string ActName => actName;

        public Action OnPlayEnd { get; set; }

        ActEntity owner;
        public ActEntity Owner { get => owner; }

        public ActEntity Target { get; set; }

        public ActAsset GetAsset { get => data; }

        public List<ActEntity> TargetList { get; set; }

        public float GetTimer { get => timer; }

        bool _isDebugMode;
        public bool IsDebugMode { get => _isDebugMode; }

        bool isUpdate = false;
        ClipState[] clipState;

        float timer;
        float deltaTime { get => Time.deltaTime * PlaySpeed; }

        bool isLoaded = false;

        public Action LoadComplete;

        HashSet<ActBaseClip> triggeredClips = new HashSet<ActBaseClip>();

        bool isDestroyed = false;

        public ActInstance(string actName = null)
        {
            this.actName = actName;
        }

        public ActInstance(ActAsset asset)
        {
            Loaded(asset);
        }

        public void Load(string actResName = null)
        {
            if (!string.IsNullOrEmpty(actResName))
                this.actName = actResName;

            if (string.IsNullOrEmpty(actName))
                return;
            isDestroyed = false;
            Facade.Externals.InstantiateAsync(actName, Loaded, false);
        }

        void Loaded(object obj)
        {
            if (isDestroyed)
                return;
            data = (ActAsset)obj;
            Refresh();
            isLoaded = true;
        }

        public void Refresh()
        {
            clipState = new ClipState[data.Clips.Count];

            for (int i = 0; i < data.Clips.Count; i++)
            {
                ActBaseClip node = data.Clips[i];
                node.ActInstance = this;

                clipState[i] = ClipState.WaitTrigger;
            }
        }

        public void Start(ActEntity entity)
        {
            timer = 0;
            isUpdate = true;
            LifeOver = false;
            this.owner = entity;
        }

        /// <summary>
        /// 开启调试模式
        /// </summary>
        public void OpenDebugMode()
        {
            _isDebugMode = true;
        }

        public void Update()
        {
            if (!isLoaded || !isUpdate || _isDebugMode)
                return;

            if (LoadComplete != null)
            {
                LoadComplete.Invoke();
                LoadComplete = null;
            }

            timer += deltaTime;

            UpdateByTime(timer);

            if (timer >= data.LifeTime)
            {
                End();
            }
        }

        public void UpdateByTime(float time)
        {
            for (int i = 0; i < data.Clips.Count; i++)
            {
                ActBaseClip clip = data.Clips[i];
                ClipState lastState = clipState[i];
                ClipState newState = GetStateByTime(clip, time);

                CheckState(clip, lastState, newState, out ClipState state, i);

                //Debug.Log($"状态检查 {ActUtility.GetDisplayName(clip.GetType())} last {lastState} new {newState} state {state} time {time}");

                clipState[i] = state;

                switch (state)
                {
                    case ClipState.WaitTrigger:
                        if (time >= clip.TriggerTime)
                        {
                            clipState[i] = ClipState.Triggered;
                            if (clip.OnTrigger())
                                triggeredClips.Add(clip);
                        }
                        break;
                    case ClipState.Triggered:
                        clip.Update(time - clip.TriggerTime, deltaTime);
                        if (clip.Duration >= 0 && time >= clip.TriggerTime + clip.Duration)
                        {
                            clip.OnDisable();
                            clipState[i] = ClipState.Disabled;
                        }
                        break;
                    case ClipState.Disabled:
                        break;
                }
            }
        }

        /// <summary>
        /// 主要针对调试模式时的倒退处理
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="lastState"></param>
        /// <param name="newState"></param>
        /// <param name="toState"></param>
        void CheckState(ActBaseClip clip, ClipState lastState, ClipState newState, out ClipState toState, int index)
        {
            toState = lastState;

            if (!_isDebugMode)
                return;

            if (lastState == newState)
                return;

            switch (newState)
            {
                case ClipState.WaitTrigger:
                    if (lastState == ClipState.Triggered)
                    {
                        clip.OnDisable();
                        toState = ClipState.WaitTrigger;
                    }
                    break;
                case ClipState.Triggered:
                    bool isTriggerd = triggeredClips.Contains(clip);
                    if (isTriggerd)
                    {
                        clip.OnEnable();
                        toState = ClipState.Triggered;
                    }
                    else
                    {
                        if (clip.OnTrigger())
                        {
                            triggeredClips.Add(clip);
                        }
                        toState = ClipState.Triggered;
                    }
                    break;
                case ClipState.Disabled:
                    if (lastState == ClipState.Triggered)
                        clip.OnDisable();
                    toState = ClipState.Disabled;
                    break;
            }
        }

        ClipState GetStateByTime(ActBaseClip clip, float time)
        {
            if (time < clip.TriggerTime)
                return ClipState.WaitTrigger;
            else if (clip.Duration < 0 || time < clip.TriggerTime + clip.Duration)
                return ClipState.Triggered;
            else
                return ClipState.Disabled;
        }

        void End()
        {
            LifeOver = true;
            isUpdate = false;
            this.OnPlayEnd?.Invoke();
        }

        public void Destroy()
        {
            isDestroyed = true;
            if (data == null)
                return;

            isUpdate = false;
            for (int i = 0; i < data.Clips.Count; i++)
            {
                data.Clips[i].Destory();
                data.Clips[i].ActInstance = null;
            }
            owner = null;
            data = null;
            triggeredClips.Clear();
        }

        public void Clear()
        {
            if (data == null)
                return;

            for (int i = 0; i < data.Clips.Count; i++)
            {
                data.Clips[i].ActInstance = null;
            }
            triggeredClips.Clear();
        }

        public void Reset()
        {
            if (data == null)
                return;

            for (int i = 0; i < data.Clips.Count; i++)
            {
                data.Clips[i].Reset();
            }
            triggeredClips.Clear();
        }
    }
}

