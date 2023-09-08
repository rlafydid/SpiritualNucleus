using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

public enum ETeamType
{
	Friendly,
	Enemy
}

[System.Serializable, NodeMenuItem("Skill/筛选/最近目标")]
public class FindNearestTargetNode : UniversalNode
{
	public override string name => "最近目标";

	[Input(name = "朝向目标")]
	public List<long> targets;
	
	[Output(name = "最近的目标")]
	public long nearestTarget;
	
	protected override void Process()
	{
		var owner = Facade.Battle.GetActor(skilUnit.OwnerID);

		float distance = float.MaxValue;
		foreach (var targetId in targets)
		{
			var target = Facade.Battle.GetActor(targetId);
			var dis = Vector3.Distance(owner.Position, target.Position);
			if (dis < distance)
			{
				distance = dis;
				nearestTarget = target.ID;
			}
		}
	}
}
