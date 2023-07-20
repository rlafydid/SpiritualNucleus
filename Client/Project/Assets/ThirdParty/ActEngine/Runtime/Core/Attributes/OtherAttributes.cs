using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act
{
     /// <summary>
     /// 从Act事件数据同步到技能蓝图节点的字段
     /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class SynchroDataAttribute : Attribute
    {

    }
}
