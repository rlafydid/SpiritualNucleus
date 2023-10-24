using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using LKEngine;
using UnityEngine;


[NodeMenuItem("技能/触发器/子弹命中")]
public class BulletHitNode : TriggerNode
{
    [Input(name = "In")]
    public ExecuteLink executed;

    public override string name => "子弹命中";
    [Output("子弹Id")]
    long bulletId;

    [Output("目标")]
    long targetId;

    protected override void Process()
    {
        base.Process();
    }

    public void SetData(long bulletId, long targetId)
    {
        this.bulletId = bulletId;
        this.targetId = targetId;
    }
}
