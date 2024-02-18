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
            asc.RegisterGameplayTagEvent(GameplayTags.GetTag("Ability.Debuff.Control.Frozen"), EGameplayTagEventType.Added, Frozen);
        }

        void Frozen(GameplayTagScriptableObject tag, EGameplayTagEventType eventType)
        {
            switch (eventType)
            {
                case EGameplayTagEventType.Added:
                    this.GetComponent<FiniteStateMachine>().TriggerEvent(EEvent.Frozen);
                    break;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            var asc = ownerActor.GetComponent<AbilitySystemCharacter>();
            asc.RemoveGameplayTagEvent(GameplayTags.GetTag("Ability.Debuff.Control.Frozen"), EGameplayTagEventType.Added, Frozen);
        }
    }
}

