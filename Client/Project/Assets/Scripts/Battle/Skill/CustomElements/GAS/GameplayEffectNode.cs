using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using AbilitySystem;
using AbilitySystem.Authoring;

[System.Serializable, NodeMenuItem("技能/GAS/GameplayEffect")]
public class GameplayEffectNode : UniversalNode, IExecuteNode
{
	[Output(name = "Input")]
	public GameplayEffectScriptableObject effect;

	public override string		name => "GameplayEffect";

	protected override void Process()
	{
		var character = this.owner.Entity.GameObject.GetComponent<AbilitySystemCharacter>();
		var sqec = character.MakeOutgoingSpec(effect);
		character.ApplyGameplayEffectSpecToSelf(sqec);
	}
}
