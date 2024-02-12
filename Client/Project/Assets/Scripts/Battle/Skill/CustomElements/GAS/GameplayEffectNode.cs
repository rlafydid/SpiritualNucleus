using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using AbilitySystem;
using AbilitySystem.Authoring;

[System.Serializable, NodeMenuItem("技能/GAS/GameplayEffect")]
public class GameplayEffectNode : BaseAsyncNode
{
	[Input(name = "Effect"), SerializeField]
	public GameplayEffectScriptableObject effect;

	[Input(name = "Actor")] public long actorId;
	
	public override string		name => "GameplayEffect";
	
	[Output(name = "End")]
	public ExecuteLink endEffect;


	private GameplayEffectSpec _gameplayEffectSpec;
	protected override void Process()
	{
		var actor = Facade.Battle.GetActor(actorId);
		var character = this.owner.GetComponent<AbilitySystemCharacter>();
		var sqec = character.MakeOutgoingSpec(effect);
		_gameplayEffectSpec = sqec;
		character.ApplyGameplayEffectSpecToSelf(sqec);
	}

	public override void Update(int deltaTime)
	{
		if (_gameplayEffectSpec.DurationRemaining <= 0)
		{
			this.Execute(endEffect);
			OnFinished();
		}
	}
}
