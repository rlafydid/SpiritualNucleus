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
		timer = 0;
	}

    public override void Update(int deltaTime)
    {
        base.Update(deltaTime);
		timer += deltaTime;
		realTimer += deltaTime;
		if (timer >= interval)
        {
			ExecuteOutputsNode();
			timer = 0;
		}

		if(realTimer >= timeLength)
        {
			OnFinished();
        }
    }

	void ExecuteOutputsNode()
    {
		foreach(var node in GetExecutedNodes())
        {
			ExecuteNode(node);
        }
    }
}
