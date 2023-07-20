using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act
{

    [Serializable]
    public class ActBaseBulletData
    {
        [ActDisplayName(DisplayName = "子弹资源", ShowNameOnly = true), DependentObject(typeof(GameObject))]
        public string bulletID = "fx_com_bullet01";

        [ActDisplayName("发射偏移值"), SynchroData]
        public Vector3 launchOffset;
    }

    [ActDisplayName("指向子弹"), Serializable]
    public class ActTraceBulletData : ActBaseBulletData
    {
        [ActDisplayName("散射次数")]
        public int dispersionNum = 1;

        [ActDisplayName("连射次数")]
        public int continuousShootingCount = 1;

        [ActDisplayName("连射时间间隔")]
        public float continuousTimeInterval = 1;

        //[ActDisplayName("应用角色属性")]
        //public bool affectedState = true;

        [ActDisplayName("立刻指向目标")]
        public bool turnToTargetNow = false;

        [ActDisplayName("速度")]
        public float speed = 1;
    }

    [ActDisplayName("矢量子弹"), Serializable]
    public class ActVectorBulletData : ActBaseBulletData
    {
        [ActDisplayName("散射次数")]
        public int dispersionNum = 1;

        [ActDisplayName("连射次数")]
        public int continuousShootingCount = 1;
        [ActDisplayName("连射时间间隔")]
        public float continuousTimeInterval = 1;
        [ActDisplayName("散射是否对称")]
        public bool BPdispersionSymmetry = true;
        [ActDisplayName("散射偏移角")]
        public float BPdispersionAngle;

        //[ActDisplayName("应用角色属性")]
        //public bool affectedState = true;

        [ActDisplayName("速度"), SynchroData]
        public float speed = 1;
    }

    [ActDisplayName("投掷子弹"), Serializable]
    public class ActMissileBulletData : ActBaseBulletData
    {
        [ActDisplayName("连射次数")]
        public int continuousShootingCount = 1;
        [ActDisplayName("连射时间间隔")]
        public float continuousTimeInterval = 1;

        //[ActDisplayName("应用角色属性")]
        //public bool affectedState = true;


        [ActDisplayName("模式（1：固定高度 2：固定横向初速度）")]
        public int type = 1;

        [ActDisplayName("横向速度Vx"), SynchroData]
        public float forwardSpeed = 1;

        [ActDisplayName("达到高度H"), SynchroData]
        public float height = 1;

        [ActDisplayName("y轴加速度g(正值)"), SynchroData]
        public float gAcceleration = 1;

        [ActDisplayName("伤害范围(半径)")]
        public float damageRadius = 1;
    }

    [ActDisplayName("链式子弹"), Serializable]
    public class ActChainBulletData : ActBaseBulletData
    {
        //[ActDisplayName("应用角色属性")]
        //public bool affectedState = true;

        [ActDisplayName("最短存活（毫秒）"), SynchroData]
        public int minLifetime = 300;

        [ActDisplayName("最长存活（毫秒）"), SynchroData]
        public int maxLifetime = 1000;

        //[ActDisplayName("连射次数")]
        public int continuousShootingCount = 1;

        [ActDisplayName("弹射次数")]
        public int catapultNum = 1;

        [ActDisplayName("弹射延时（毫秒）")]
        public int catapultDelay;


        [ActDisplayName("起始挂点"), SynchroData]
        public string startAttachedPoint = "Muzzle";

        [ActDisplayName("结束挂点"), SynchroData]
        public string endAttachedPoint = "RoleCenter";


    }


}
