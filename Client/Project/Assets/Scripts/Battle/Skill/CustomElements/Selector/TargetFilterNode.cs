using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable]
public class TargetFilterNode : UniversalNodeWithOut
{
	[Output(name = "列表")]
	public List<long> selectTargets;

	protected override void Process()
	{

	}
}
