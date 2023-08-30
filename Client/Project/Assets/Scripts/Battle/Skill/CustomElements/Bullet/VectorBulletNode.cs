using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using Battle;

[System.Serializable, NodeMenuItem("弹道/矢量子弹")]
public class VectorBulletNode : BaseBulletNode
{
	public override string		name => "矢量子弹";

	[Input("偏移角"), SerializeField]
	public float angle;

	[Input("距离"), SerializeField]
	public float distance = 20;

	[Input("数量"), SerializeField]
	public int count = 1;
	
	protected override void Process()
	{
		var data = new VectorBulletData();
		data.res = res;
		data.angle = angle;
		data.speed = speed;
		data.dispersionCount = count;
		data.offset = offset;
		data.distance = distance;
		data.hitTriggerNode = GetHitTriggerNodes().ToList();
		data.shooter = owner.ID;
		data.process = this.process;
		Debug.Log("触发节点数量" + data.hitTriggerNode.Count);
		Facade.Bullet.LaunchBullet(data);
	}
}
