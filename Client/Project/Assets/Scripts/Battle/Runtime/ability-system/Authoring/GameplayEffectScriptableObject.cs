using System.Collections;
using System.Collections.Generic;
using Act;
using UnityEngine;
using UnityEngine.Serialization;

namespace AbilitySystem.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Gameplay Effect Definition")]
    public class GameplayEffectScriptableObject : ScriptableObject
    {
        [SerializeField]
        public GameplayEffectDefinitionContainer gameplayEffect;

        [SerializeField]
        public GameplayEffectTags gameplayEffectTags;

        [FormerlySerializedAs("Period")] [SerializeField]
        public GameplayEffectPeriod period;
    }

}
