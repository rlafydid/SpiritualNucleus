using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act
{
    [ActDisplayName("动画", "播放动画"), Serializable]
    public class ActAnimationClip : ActBaseClip
    {
        public string animName;
        public bool dontStopOnDisable = false;
        //public float speed = 1;


        public override bool SupportResize => false;

        public override void Preload()
        {
            base.Preload();
        }

        SimpleAnimation simpleAnimation;
        SimpleAnimation.State animState;
        Animator animator;

        public override bool OnTrigger()
        {
            if (string.IsNullOrEmpty(animName))
            {
                Debug.LogError($"{ActInstance.ActName}中请配置动画");
                return false;
            }
            simpleAnimation = owner.GetMonoComponentInChildren<SimpleAnimation>();
            if (simpleAnimation != null)
            {
                animState = simpleAnimation.GetState(animName);
                if (animState == null)
                {
                    Debug.LogError($"没找到{animName}对应的动作");
                    return true;
                }

                simpleAnimation.Stop(animName);
                if (dontStopOnDisable)
                    simpleAnimation.CrossFade(animName, 0.2f, false);
                else
                    simpleAnimation.CrossFade(animName, 0.2f, false);
                animState.speed = ActInstance.PlaySpeed;
                //Debug.Log($"ACT stateName {animName} length {animState.length} isplaying = {simpleAnimation.IsPlaying(animName)} speed {animState.speed}");
                Duration = animState.length;
            }
            else
            {
                animator = owner.GetMonoComponentInChildren<Animator>();
                //foreach (var anim in animator.parameters)
                //{
                //    if (anim.defaultBool)
                //    {
                //        Debug.Log($"哈哈 重置状态 {anim.name}");
                //        animator.ResetTrigger(anim.name);
                //    }
                //}

                animator.SetTrigger(animName);
                animator.speed = ActInstance.PlaySpeed;
            }
            return true;
        }

        public override void Update(float time, float deltaTime)
        {
            if (simpleAnimation == null)
                return;

            if (animState == null)
                animState = simpleAnimation.GetState(animName);

            if (animState == null)
                return;

            if (isDebugMode)
            {
                if (time >= animState.clip.length - Time.fixedDeltaTime - 0.01f)
                    return;

                if (simpleAnimation.getPlayingState.name != animName)
                {
                    //Debug.Log($"stateName {animName} time {time} TriggerTime {TriggerTime} length {animState.clip.length} playingState {simpleAnimation.getPlayingState.name}");
                    simpleAnimation.Play(animName, !isDebugMode);
                }
                animState.speed = 0;
                animState.time = time;
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            if (dontStopOnDisable)
                return;

            //if (simpleAnimation != null)
            //{
            //    simpleAnimation.Stop(animName);
            //}
            //else
            //{
            //    Animator animator = owner.GetMonoComponentInChildren<Animator>();
            //    animator.SetBool(animName, false);
            //}
        }

        public override void OnEnable()
        {
            if (simpleAnimation != null && isDebugMode)
                simpleAnimation.Play(animName, !isDebugMode);
        }

        public override void ChangeSpeed(float speed)
        {
            base.ChangeSpeed(speed);
            if (animState != null)
                animState.speed = speed;
            else if (animator != null)
            {
                animator.speed = speed;
            }
        }

        public override void Destory()
        {
            base.Destory();
        }
    }
}
