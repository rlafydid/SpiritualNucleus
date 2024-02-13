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
	
	[Output(name = "Actor")] public long outputActorId;
	
	public override string		name => "GameplayEffect";
	
	[Output(name = "End")]
	public ExecuteLink endEffect;

	private List<GameplayEffectSpec> _specs = new();
	
	protected override void Process()
	{
		outputActorId = actorId;
		Debug.Log($"effect to actor id  {actorId}");
		var actor = Facade.Battle.GetActor(actorId);
		var character = actor.GetComponent<AbilitySystemCharacter>();
		var sqec = character.MakeOutgoingSpec(effect);
		_specs.Add(sqec);
		character.ApplyGameplayEffectSpecToSelf(sqec);
	}

	public override void Update(int deltaTime)
	{
		for (int i = _specs.Count - 1; i >= 0; i--)
		{
			var spec = _specs[i];
			if (spec.DurationRemaining <= 0)
			{
				outputActorId = spec.Source.OwnerId;
				this.Execute(nameof(endEffect));
				_specs.RemoveAt(i);
				Debug.Log($"移除Effect {i}");
			}
		}
		if (_specs.Count <= 0)
		{
			OnFinished();
		}
	}
}
