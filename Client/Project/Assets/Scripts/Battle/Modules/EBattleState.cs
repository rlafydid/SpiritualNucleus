using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public enum ERoleState
    {
        Idle, //待机
        Move, //移动

        Attack, //攻击

        Evade, //躲避
        Jump, //跳跃

        Hurt, //受伤
        KnockBack, //击退
        KnockFly, //击飞

        FlashMove, //疾驰

        Vertigo, //眩晕

        Frozen, //冰冻

        TraceTarget, //追踪目标
        Dead, //死亡
    }
}
