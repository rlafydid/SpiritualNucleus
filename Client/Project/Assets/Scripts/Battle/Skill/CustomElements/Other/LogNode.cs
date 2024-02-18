using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

[NodeMenuItem("打印")]
public class LogNode : UniversalNode
{
    [Input("打印"), SerializeField]
    public string log;
    protected override void Process()
    {
        base.Process();
        Debug.Log(log);
    }
}
