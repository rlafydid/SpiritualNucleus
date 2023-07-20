/*================================
 * 类名称: Act访问外部接口
 * 类描述: 
 * 目的: 
 * 创建人: Loong
 * 创建时间:
 * 修改人:
 * 修改时间:
 * 版本: @version 1.0
==================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act.Facade
{
    /// <summary>
    /// 调用外部接口
    /// </summary>
    public class Externals
    {
        public static Action<string, Action<UnityEngine.Object>> LoadAssetAsync;
        public static Action<UnityEngine.Object> ReleaseAsset;
        public static Action<string, Action<UnityEngine.Object>, bool/* 是否是Prefab */> InstantiateAsync;
        public static Action<UnityEngine.Object> ReleaseInstance;
        public static Action<string, string, string> PlayAudio;
        public static Func<string, string, float> GetAudioTime;

        public static Func<GameObject, float> GetParticleSystemDuration;

        public static Func<string, int, string> GetResourceName;

        public static Action<float, Action> Delay;
    }

    /// <summary>
    /// 预览相关
    /// </summary>
    public class Preview
    {
        /// <summary>
        /// 获取预览主角
        /// </summary>
        public static Func<GameObject> GetMainCharactor;
        /// <summary>
        /// 获取靶子
        /// </summary>
        public static Func<GameObject> GetTarget;
        /// <summary>
        /// 受击Act
        /// </summary>
        public static Func<ActAsset> GetHitAct;

        /// <summary>
        /// 重新加载预览
        /// </summary>
        public static Action ReloadPreview;

        public static Action<string, float> PlayAct;
        public static Action StopAct;

        public static Action<ActBaseEvent> TriggerEvent;
    }

    /// <summary>
    /// 内部接口
    /// </summary>
    public class Internals
    {
        public class Timeline
        {
            /* 预览 ---控制---> ActTimeline */
            public static Action FixedUpdate;
            public static Action AddFrame;
            public static Action Reset;

            /* ActTimeline  ---控制---> 预览 */
            public static Action<float> Simulate;
            public static Action OpenDebugMode;
        }
    }


}
