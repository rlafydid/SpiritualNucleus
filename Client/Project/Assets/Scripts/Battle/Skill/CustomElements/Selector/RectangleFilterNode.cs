using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using Battle;

[System.Serializable, NodeMenuItem("技能/筛选目标/矩形")]
public class RectangleFilterNode : TargetFilterNode
{
	
	[Input("宽度"), SerializeField]
	float width;
	
	[Input("高度"), SerializeField]
	float height;

	[Input("偏移值"), SerializeField]
	Vector3 offset;

	public override string		name => "筛选矩形内目标";

	protected override void Process()
	{
		selectTargets.Clear();
		var owner = Facade.Battle.GetActor(skilUnit.OwnerID);

		var targets = owner is Battle.HeroActorController ?  Facade.Battle.GetMonsters() : Facade.Battle.GetHeroActors();
		for (int i = 0; i < targets.Count; i++)
		{
			var target = targets[i];
			if (IsInRectangle(owner, target.Position))
			{
				Debug.Log("在范围内");
				selectTargets.Add(target.ID);
			}
		}
	}

	bool IsInRectangle(SceneActorController owner, Vector3 point)
	{
		Vector3 center = owner.Entity.Position + owner.Entity.LocalRotation * offset;

		float halfWidth = width * 0.5f;
		float halfHeight = height * 0.5f;
		//左下，左上，右上，右下
		Vector3 p1 =  new Vector3(-halfWidth, 0, -halfHeight);
		Vector3 p2 =  new Vector3(-halfWidth, 0, halfHeight);
		Vector3 p3 = new Vector3(halfWidth, 0, halfHeight);
		Vector3 p4 = new Vector3(halfWidth, 0, -halfHeight);

		point.y = 0;
		Vector3 targetOffset = point - owner.Entity.Position;
		
		Vector3[] rectangle = { p1, p2, p3, p4 };

		for (int i = 1; i <= rectangle.Length; i++)
		{
			Vector3 lastP = center + owner.Entity.LocalRotation * rectangle[i-1];
			Vector3 curP = center + owner.Entity.LocalRotation * rectangle[i == rectangle.Length ? 0 : i];
			if (Vector3.Dot((curP - lastP).normalized, (point - lastP).normalized) < 0)
			{
				return false;
			}
		}

		return true;
	}

	// bool IsInLine(Vector3 pos, Vector3 directory, Vector3 targetPoint)
 //    {
 //
	// 	Vector2 p1 = pos.To2D();
	// 	Vector2 p2 = (pos + directory * 3).To2D();
 //
 //
	// 	Vector2 p = targetPoint.To2D();
 //
	// 	if(Vector2.Dot(directory.normalized, (targetPoint - pos).normalized) < 0)
	// 	{
	// 		return false;
	// 	}
 //
	// 	float distance = Vector2.Distance(p1, p);
 //
	// 	if (p1.x == p2.x)
	// 	{
	// 		return Mathf.Abs(p.x - p1.x) < angleRange && distance <= distanceRange;
	// 	}
 //
 //
	// 	Vector2 dir = p2 - p1;
 //
	// 	float k = (p2.y - p1.y) / (p2.x - p1.x);
	// 	float t = directory.y - k * directory.x;
 //
	// 	float a = k;
	// 	float b = -1;
	// 	float c = (p2.x * p1.y - p1.x * p2.y)/(p2.x - p1.x);
 //
	// 	c = p2.y - k * p2.x;
 //
	// 	float p2LineDistance = Mathf.Abs(a * p.x + b * p.y + c) / (Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(b, 2)));
 //
	// 	return p2LineDistance < angleRange && distance < distanceRange;
	// }
}
