using System.Collections;
using System.Collections.Generic;
using AbilitySystem.ModifierMagnitude;
using UnityEngine;

namespace AbilitySystem
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Gameplay Effect/Modifier Magnitude/Test")]
    public class TestModifierMagnitude : ModifierMagnitudeScriptableObject
    {
        public override void Initialise(GameplayEffectSpec spec)
        {
        }
        public override float? CalculateMagnitude(GameplayEffectSpec spec)
        {
            return 1;
        }
    }
}
