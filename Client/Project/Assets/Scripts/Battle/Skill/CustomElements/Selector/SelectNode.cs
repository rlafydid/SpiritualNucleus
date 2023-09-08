using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;

[System.Serializable, NodeMenuItem("Skill/筛选/筛选目标")]
public class SelectNode : UniversalNode
{
	enum RangeType
	{
		Sector,
		Line
	}

	[Output(name = "列表")]
	public List<long> selectTargets;

	[Input("RangeType"), SerializeField]
	RangeType rangeType;

	[Input("AngleRange"), SerializeField]
	int angleRange;

	[Input("DistanceRange"), SerializeField]
	float distanceRange;

	public override string		name => "筛选目标";

	protected override void Process()
	{
		selectTargets.Clear();
		var owner = Facade.Battle.GetActor(skilUnit.OwnerID);

		var targets = owner is Battle.HeroActorController ?  Facade.Battle.GetMonsters() : Facade.Battle.GetHeroActors();
		for (int i = 0; i < targets.Count; i++)
		{
			var actor = targets[i];
			switch(rangeType)
            {
				case RangeType.Sector:
					if (IsInSector(owner.Entity.Forward, actor.Position - owner.Position) && Vector3.Distance(owner.Position.To2D(), actor.Position.To2D()) < distanceRange)
					{
						selectTargets.Add(actor.ID);
					}
					break;
				case RangeType.Line:
					if (IsInLine(owner.Entity.Position, owner.Entity.Forward, actor.Position))
					{
						selectTargets.Add(actor.ID);
					}
					break;
			}
		}
	}

	bool IsInSector(Vector3 pos, Vector3 targetPos)
	{
		return Vector3.Angle(pos.To2D(), targetPos.normalized.To2D()) * 0.5 <= angleRange;
	}

	bool IsInLine(Vector3 pos, Vector3 directory, Vector3 targetPoint)
    {

		Vector2 p1 = pos.To2D();
		Vector2 p2 = (pos + directory * 3).To2D();


		Vector2 p = targetPoint.To2D();

		if(Vector2.Dot(directory.normalized, (targetPoint - pos).normalized) < 0)
		{
			return false;
		}

		float distance = Vector2.Distance(p1, p);

		if (p1.x == p2.x)
		{
			return Mathf.Abs(p.x - p1.x) < angleRange && distance <= distanceRange;
		}


		Vector2 dir = p2 - p1;

		float k = (p2.y - p1.y) / (p2.x - p1.x);
		float t = directory.y - k * directory.x;

		float a = k;
		float b = -1;
		float c = (p2.x * p1.y - p1.x * p2.y)/(p2.x - p1.x);

		c = p2.y - k * p2.x;

		float p2LineDistance = Mathf.Abs(a * p.x + b * p.y + c) / (Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2)));

		return p2LineDistance < angleRange && distance < distanceRange;
	}
}
