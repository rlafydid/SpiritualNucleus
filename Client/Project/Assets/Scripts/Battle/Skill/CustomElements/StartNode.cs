using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Custom/StartNode")]
public class StartNode : BaseSkillNode, IExecuteNode
{
	[Output(name = "Out")]
	public ExecuteLink executes;

	public override string		name => "StartNode";

	public IEnumerable<BaseSkillNode> GetExecutedNodes()
	{
		// Return all the nodes connected to the executes port
		return outputPorts.FirstOrDefault(n => n.fieldName == nameof(executes))
			.GetEdges().Select(e => e.inputNode as BaseSkillNode);
	}

	protected override void Process()
	{
	}
}
