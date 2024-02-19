using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using GameplayTag.Authoring;
using UnityEngine;

namespace Battle
{
    public class FrozenState : MonsterState<KnockBackData>
    {
        private GameplayTagScriptableObject frozenTag;
        protected override void OnEnter()
        {
            base.OnEnter();
            var abilitySystem = GetActor.GetComponent<AbilitySystemCharacter>();
            abilitySystem.RegisterGameplayTagEvent(GameplayTags.GetTag("Ability.Debuff.Control.Frozen"), RemovedTag);
            
            this.GetActor.PlayAct("Act_Buff_Frozen");
        }

        void RemovedTag(GameplayTagScriptableObject tag, EGameplayTagEventType type)
        {
            ToLinkedState();
        }
        
        protected override void OnExit()
        {
            base.OnExit();
            var abilitySystem = GetActor.GetComponent<AbilitySystemCharacter>();
            abilitySystem.RemoveGameplayTagEvent(GameplayTags.GetTag("Ability.Debuff.Control.Frozen"), RemovedTag);
        }
    }
}
