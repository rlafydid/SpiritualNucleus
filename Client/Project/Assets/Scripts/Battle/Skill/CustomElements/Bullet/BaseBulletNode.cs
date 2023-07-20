using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using UnityEngine;


public class BaseBulletNode : UniversalNode
{
	[Input("资源"), SerializeField]
	public string res;

	[Input("偏移值"), SerializeField]
	public Vector3 offset;

	[Input("速度"), SerializeField]
	public float speed;

	[Output("触发命中")]
	public ExecuteLink hitTriggerNodes;

	public IEnumerable<TriggerNode> GetHitTriggerNodes()
	{
		// Return all the nodes connected to the executes port
		return outputPorts.FirstOrDefault(n => n.fieldName == nameof(hitTriggerNodes))
			.GetEdges().Select(e => e.inputNode as TriggerNode);
	}
}
