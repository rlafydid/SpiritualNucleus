using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Skill/Delay")]
public class DelayNode : BaseAsyncNode
{
	[Input(name = "In")]
    public float                input;

	[Output(name = "Out")]
	public float				output;

	public override string		name => "Delay";

	public float delayTime;

	protected override void Process()
	{
		TimerMod.Delay(delayTime, OnFinished);
	}
}
