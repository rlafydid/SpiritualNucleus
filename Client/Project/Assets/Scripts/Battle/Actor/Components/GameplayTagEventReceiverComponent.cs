using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using FSM;
using GameplayTag.Authoring;
using UnityEngine;

namespace Battle
{
    public class ActorStateTagComponent: ActorComponent
    {
        protected override void OnStart()
        {
            base.OnStart();
            var asc = ownerActor.GetComponent<AbilitySystemCharacter>();
            asc.OnGameplayTagChanged += OnGameplayTagChanged;
        }

        void OnGameplayTagChanged(GameplayTagScriptableObject tag, EGameplayTagEventType eventType)
        {
            if (eventType == EGameplayTagEventType.Added)
            {
                switch (tag.name)
                {
                    case "Ability.Debuff.Control.Frozen":
                        this.ownerActor.GetComponent<FiniteStateMachine>().TriggerEvent(EEvent.Frozen);
                        break;
                }
            } 
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            var asc = ownerActor.GetComponent<AbilitySystemCharacter>();
            asc.OnGameplayTagChanged -= OnGameplayTagChanged;
        }
    }
}

