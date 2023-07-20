using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using LKEngine;
using UnityEngine;

namespace Facade
{
    public class TriggerNode
    {
        public static Action<long, Entity> BulletHit;
    }
}

public class TriggerNode : BaseSkillNode,IExecuteNode
{
	[Output(name = "Out")]
	public ExecuteLink executes;

	public IEnumerable<BaseSkillNode> GetExecutedNodes()
	{
		// Return all the nodes connected to the executes port
		return outputPorts.FirstOrDefault(n => n.fieldName == nameof(executes))
			.GetEdges().Select(e => e.inputNode as BaseSkillNode);
	}
}