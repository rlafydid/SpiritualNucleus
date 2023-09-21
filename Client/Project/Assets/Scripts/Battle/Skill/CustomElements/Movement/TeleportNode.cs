using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

[System.Serializable, NodeMenuItem("位移相关/瞬移")]
public class TeleportNode : UniversalNode
{
    public override string name => "瞬移";

    [Input(name = "目标位置")]
    public Vector3 target;
    [Input(name = "偏移值"), SerializeField]
    public Vector3 offset;

    protected override void Process()
    {
        base.Process();
        owner.Position = target + offset;
    }
}
