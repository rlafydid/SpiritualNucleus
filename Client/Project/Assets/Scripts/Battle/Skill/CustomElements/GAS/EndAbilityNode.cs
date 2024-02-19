using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;

[System.Serializable, NodeMenuItem("技能/GAS/EndAbility")]
public class EndAbilityNode : UniversalNode
{
	public override string		name => "EndAbility";
	
	private List<GameplayEffectNodeItem> _items = new();

	protected override void Process()
	{
		skilUnit.EndAbility();
	}
}
