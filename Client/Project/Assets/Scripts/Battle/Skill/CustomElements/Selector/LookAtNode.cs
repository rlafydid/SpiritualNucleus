using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("技能/朝向/朝向目标")]
public class LookAtNode : UniversalNodeWithOut
{
	public override string name => "朝向目标";

	[Input(name = "朝向目标")]
	public long toTarget;

	protected override void Process()
	{
		var owner = Facade.Battle.GetActor(skilUnit.OwnerID);
		var target = Facade.Battle.GetActor(toTarget);
		if (target == null)
			return;
		var targetPos = target.Position;
		targetPos.y = owner.Position.y;
		owner.Entity.LookAt(targetPos);
	}
}
