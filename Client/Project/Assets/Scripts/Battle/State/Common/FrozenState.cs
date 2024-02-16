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
            // frozenTag = abilitySystem.GetTag("Frozen");
            // abilitySystem.RegisterGameplayTagEvent(frozenTag, EGameplayTagEventType.Removed, RemovedTag);
        }

        void RemovedTag(GameplayTagScriptableObject tag, EGameplayTagEventType type)
        {
        }
        
        protected override void OnExit()
        {
            base.OnExit();
        }
    }
}
