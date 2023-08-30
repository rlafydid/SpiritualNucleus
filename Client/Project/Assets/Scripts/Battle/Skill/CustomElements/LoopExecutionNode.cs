using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("节点相关/循环执行")]
public class LoopExecutionNode : BaseAsyncNode
{
	public override string name => "循环执行";

	[Input("时间间隔"), SerializeField]
	long interval;

	[Input("总时间"), SerializeField]
	long timeLength;

	long timer;
	long realTimer;
	protected override void Process()
	{
		realTimer = 0;
		timer = 0;
		Debug.Log($"LoopExecutionNode Process");
	}

    public override void Update(int deltaTime)
    {
        base.Update(deltaTime);
		timer += deltaTime;
		realTimer += deltaTime;
		Debug.Log($"LoopExecutionNode Process Tine {realTimer}");
		if (timer >= interval)
        {
	        Debug.Log($"LoopExecutionNode  timer {realTimer}");
			ExecuteOutputsNode();
			timer = 0;
		}

		if(realTimer >= timeLength)
        {
	        Debug.Log($"LoopExecutionNode  timer {realTimer} timeLength {timeLength} 结束 ");
			OnFinished();
        }
    }

	void ExecuteOutputsNode()
    {
	    Debug.Log($"执行Node {realTimer}");
		foreach(var node in GetExecutedNodes())
        {
			ExecuteNode(node);
        }
    }
}
