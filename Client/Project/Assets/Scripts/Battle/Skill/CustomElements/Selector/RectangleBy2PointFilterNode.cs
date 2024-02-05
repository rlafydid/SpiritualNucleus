using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using Battle;

[System.Serializable, NodeMenuItem("技能/筛选目标/矩形(根据两点形成矩形)")]
public class RectangleBy2PointFilterNode : TargetFilterNode
{
	[Input("宽度"), SerializeField]
	float width;

	[Input("起始点"), SerializeField]
	public Vector3 startPoint;

	[Input("终点"), SerializeField]
	public Vector3 endPoint;

	public override string		name => "筛选根据两点矩形内目标";

	protected override void Process()
	{
		selectTargets.Clear();
		var owner = Facade.Battle.GetActor(skilUnit.OwnerID);

		float height = Vector3.Distance(startPoint, endPoint);

		Vector3 center = (startPoint + endPoint) / 2;

		Quaternion rotation = Quaternion.LookRotation((endPoint - startPoint).normalized);
		
		var targets = owner is Battle.HeroActorController ?  Facade.Battle.GetMonsters() : Facade.Battle.GetHeroActors();
		for (int i = 0; i < targets.Count; i++)
		{
			var target = targets[i];
			if (MathUtils.IsInRectangle(width, height, rotation, center, target.Position))
			{
				Debug.Log("在范围内");
				selectTargets.Add(target.ID);
			}
		}
	}

	
}
