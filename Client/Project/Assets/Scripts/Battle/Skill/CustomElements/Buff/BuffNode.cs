using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

public class BuffNode : UniversalNodeWithOut
{
    public override string name => "Buff";
    [Input("单个目标")]
    protected long target;
    [Input("目标集合")]
    protected List<long> targets;
    [Input("持续时间")]
    protected long duration;
}
