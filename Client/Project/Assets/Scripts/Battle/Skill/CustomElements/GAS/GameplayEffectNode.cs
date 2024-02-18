using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GraphProcessor;
using System.Linq;
using AbilitySystem;
using AbilitySystem.Authoring;
using FSM;
using GameplayTag.Authoring;

public class GameplayEffectNodeItem
{
	public long ActorId { get; }
	Action<GameplayEffectNodeItem> _onFinish;
	public  GameplayEffectNodeItem(long actorId, GameplayEffectScriptableObject effect, Action<GameplayEffectNodeItem> onFinish)
	{
		var actor = Facade.Battle.GetActor(actorId);
		var asc = actor.GetComponent<AbilitySystemCharacter>();
		var sqec = asc.MakeOutgoingSpec(effect);
		asc.ApplyGameplayEffectSpecToSelf(sqec);
		asc.OnGameplayEffectRemoved += OnGameplayEffectRemoved;

		this._onFinish = onFinish;
		ActorId = actorId;
	}

	void OnGameplayEffectRemoved(AbilitySystemCharacter asc, GameplayEffectSpec effect)
	{
		asc.OnGameplayEffectRemoved -= OnGameplayEffectRemoved;
		_onFinish?.Invoke(this);
	}
}

[System.Serializable, NodeMenuItem("技能/GAS/GameplayEffect")]
public class GameplayEffectNode : BaseAsyncNode
{
	[Input(name = "Effect"), SerializeField]
	public GameplayEffectScriptableObject effect;

	[Input(name = "Actor")] public long actorId = 0;
	
	[Output(name = "Actor")] public long outputActorId;
	
	public override string		name => "GameplayEffect";
	
	[Output(name = "End")]
	public ExecuteLink endEffect;

	private List<GameplayEffectNodeItem> _items = new();

	protected override void Process()
	{
		if (actorId > 0)
		{
			outputActorId = actorId;
			Debug.Log($"effect to actor id  {actorId}");

			var item = new GameplayEffectNodeItem(actorId, effect, OnFinished);
			_items.Add(item);
		}
	}

	void OnFinished(GameplayEffectNodeItem item)
	{
		_items.Remove(item);
		outputActorId = actorId;
		Execute(nameof(endEffect));
		if (_items.Count <= 0)
			OnFinished();
	}

}
