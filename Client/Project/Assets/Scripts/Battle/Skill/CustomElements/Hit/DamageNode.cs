using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;


[System.Serializable, NodeMenuItem("技能/击中效果/伤害")]
public class DamageNode : UniversalNode
{
	public override string name => "伤害";

	[Input("目标集合")]
	List<long> selectTargets =new List<long>();

	Battle.DamageData data = new Battle.DamageData();
	protected override void Process()
	{
		base.Process();
		data.value = owner.GetComponent<Battle.AttributesComponent>().attack;

		//AudioManager.Instance.PlayAudio("baixiaofei01_attack_liuxingyu_hit.wav");
		foreach(var targetId in selectTargets)
        {
			var actor = Facade.Battle.GetActor(targetId);
			actor?.Hurt(data);
        }
	}

}
