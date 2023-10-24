using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using Battle;

[System.Serializable, NodeMenuItem("技能/筛选目标/扇形")]
public class SectorFilterNode : TargetFilterNode
{
	[Input("半径"), SerializeField] 
	private float radius = 2;
	
	[Input("角度"), SerializeField]
	float angleRange;

	public override string		name => "筛选扇形内目标";

	protected override void Process()
	{
		selectTargets.Clear();
		var owner = Facade.Battle.GetActor(skilUnit.OwnerID);

		var targets = owner is Battle.HeroActorController ?  Facade.Battle.GetMonsters() : Facade.Battle.GetHeroActors();
		for (int i = 0; i < targets.Count; i++)
		{
			var target = targets[i];
			if (Vector3.Distance(owner.Position, target.Position) <= radius && IsInSector(owner.Position, target.Position))
			{
				selectTargets.Add(target.ID);
			}
		}
	}

	bool IsInSector(Vector3 pos, Vector3 targetPos)
	{
		return Vector3.Angle(pos.To2D(), targetPos.normalized.To2D()) * 0.5 <= angleRange;
	}
}
