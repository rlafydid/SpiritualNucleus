using System;
using System.Collections.Generic;
using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using Battle;
using GameplayTag.Authoring;
using LKEngine;
using UnityEngine;


namespace AbilitySystem
{
    public class AbilitySystemCharacter : ActorComponent
    {
        public struct GameplayTagEventItem
        {
            public GameplayTagEventDelegate tagEvent;
            public EGameplayTagEventType eventType;
        }
        [SerializeField]
        protected AttributeSystemComponent _attributeSystem;
        public AttributeSystemComponent AttributeSystem { get { return _attributeSystem; } set { _attributeSystem = value; } }
        public List<GameplayEffectContainer> AppliedGameplayEffects = new List<GameplayEffectContainer>();
        public List<AbstractAbilitySpec> GrantedAbilities = new List<AbstractAbilitySpec>();
        public float Level;

        public long OwnerId { get => ownerActor.ID;}

        public delegate void GameplayTagEventDelegate(GameplayTagScriptableObject tag, EGameplayTagEventType eventType);

        private Dictionary<GameplayTagScriptableObject, List<GameplayTagEventItem>> gameplayTagEvents = new();

        public Action<AbilitySystemCharacter, GameplayEffectSpec> OnGameplayEffectApplied { get; set; }
        public Action<AbilitySystemCharacter, GameplayEffectSpec> OnGameplayEffectRemoved { get; set; }

        public GameplayTagEventDelegate OnGameplayTagChanged { get; set; }

        protected override void OnStart()
        {
            base.OnStart();
            _attributeSystem = this.ownerActor.GetComponent<AttributeSystemComponent>();
            var abilityController = this.GetActor.Entity.GameObject.GetComponent<AbilityController>();
            abilityController.InitialisaAbilites(this);
            
            
        }

        public void GrantAbility(AbstractAbilitySpec spec)
        {
            this.GrantedAbilities.Add(spec);
        }

        public void RemoveAbilitiesWithTag(GameplayTagScriptableObject tag)
        {
            for (var i = GrantedAbilities.Count - 1; i >= 0; i--)
            {
                if (GrantedAbilities[i].Ability.AbilityTags.AssetTag == tag)
                {
                    GrantedAbilities.RemoveAt(i);
                }
            }
        }


        /// <summary>
        /// Applies the gameplay effect spec to self
        /// </summary>
        /// <param name="geSpec">GameplayEffectSpec to apply</param>
        public bool ApplyGameplayEffectSpecToSelf(GameplayEffectSpec geSpec)
        {
            if (geSpec == null) return true;
            bool tagRequirementsOK = CheckTagRequirementsMet(geSpec);

            if (tagRequirementsOK == false) return false;


            switch (geSpec.GameplayEffect.gameplayEffect.DurationPolicy)
            {
                case EDurationPolicy.HasDuration:
                case EDurationPolicy.Infinite:
                    ApplyDurationalGameplayEffect(geSpec);
                    break;
                case EDurationPolicy.Instant:
                    ApplyInstantGameplayEffect(geSpec);
                    return true;
            }

            OnGameplayEffectApplied?.Invoke(this, geSpec);

            foreach (var tag in geSpec.GameplayEffect.gameplayEffectTags.GrantedTags)
            {
                OnGameplayTagChanged?.Invoke(tag, EGameplayTagEventType.Added);
                if (gameplayTagEvents.TryGetValue(tag, out var tagEvent))
                {
                    InvokeTagEvent(tag, EGameplayTagEventType.Added);
                }
            }
            
            return true;
        }
        public GameplayEffectSpec MakeOutgoingSpec(GameplayEffectScriptableObject GameplayEffect, float? level = 1f)
        {
            level = level ?? this.Level;
            return GameplayEffectSpec.CreateNew(
                GameplayEffect: GameplayEffect,
                Source: this,
                Level: level.GetValueOrDefault(1));
        }

        bool CheckTagRequirementsMet(GameplayEffectSpec geSpec)
        {
            /// Build temporary list of all gametags currently applied
            var appliedTags = new List<GameplayTagScriptableObject>();
            for (var i = 0; i < AppliedGameplayEffects.Count; i++)
            {
                appliedTags.AddRange(AppliedGameplayEffects[i].spec.GameplayEffect.gameplayEffectTags.GrantedTags);
            }

            // Every tag in the ApplicationTagRequirements.RequireTags needs to be in the character tags list
            // In other words, if any tag in ApplicationTagRequirements.RequireTags is not present, requirement is not met
            for (var i = 0; i < geSpec.GameplayEffect.gameplayEffectTags.ApplicationTagRequirements.RequireTags.Length; i++)
            {
                if (!appliedTags.Contains(geSpec.GameplayEffect.gameplayEffectTags.ApplicationTagRequirements.RequireTags[i]))
                {
                    return false;
                }
            }

            // No tag in the ApplicationTagRequirements.IgnoreTags must in the character tags list
            // In other words, if any tag in ApplicationTagRequirements.IgnoreTags is present, requirement is not met
            for (var i = 0; i < geSpec.GameplayEffect.gameplayEffectTags.ApplicationTagRequirements.IgnoreTags.Length; i++)
            {
                if (appliedTags.Contains(geSpec.GameplayEffect.gameplayEffectTags.ApplicationTagRequirements.IgnoreTags[i]))
                {
                    return false;
                }
            }

            return true;
        }

        void ApplyInstantGameplayEffect(GameplayEffectSpec spec)
        {
            for (var i = 0; i < spec.GameplayEffect.gameplayEffect.Modifiers.Length; i++)
            {
                var modifier = spec.GameplayEffect.gameplayEffect.Modifiers[i];
                var magnitude = (modifier.ModifierMagnitude.CalculateMagnitude(spec) * modifier.Multiplier).GetValueOrDefault();
                var attribute = modifier.Attribute;
                this.AttributeSystem.GetAttributeValue(attribute, out var attributeValue);

                switch (modifier.ModifierOperator)
                {
                    case EAttributeModifier.Add:
                        attributeValue.BaseValue += magnitude;
                        break;
                    case EAttributeModifier.Multiply:
                        attributeValue.BaseValue *= magnitude;
                        break;
                    case EAttributeModifier.Override:
                        attributeValue.BaseValue = magnitude;
                        break;
                }
                this.AttributeSystem.SetAttributeBaseValue(attribute, attributeValue.BaseValue);
            }
        }
        void ApplyDurationalGameplayEffect(GameplayEffectSpec spec)
        {
            var modifiersToApply = new List<GameplayEffectContainer.ModifierContainer>();
            for (var i = 0; i < spec.GameplayEffect.gameplayEffect.Modifiers.Length; i++)
            {
                var modifier = spec.GameplayEffect.gameplayEffect.Modifiers[i];
                var magnitude = (modifier.ModifierMagnitude.CalculateMagnitude(spec) * modifier.Multiplier).GetValueOrDefault();
                var attributeModifier = new AttributeModifier();
                switch (modifier.ModifierOperator)
                {
                    case EAttributeModifier.Add:
                        attributeModifier.Add = magnitude;
                        break;
                    case EAttributeModifier.Multiply:
                        attributeModifier.Multiply = magnitude;
                        break;
                    case EAttributeModifier.Override:
                        attributeModifier.Override = magnitude;
                        break;
                }
                modifiersToApply.Add(new GameplayEffectContainer.ModifierContainer() { Attribute = modifier.Attribute, Modifier = attributeModifier });
            }
            AppliedGameplayEffects.Add(new GameplayEffectContainer() { spec = spec, modifiers = modifiersToApply.ToArray() });
        }

        void UpdateAttributeSystem()
        {
            // Set Current Value to Base Value (default position if there are no GE affecting that atribute)


            for (var i = 0; i < this.AppliedGameplayEffects.Count; i++)
            {
                var modifiers = this.AppliedGameplayEffects[i].modifiers;
                for (var m = 0; m < modifiers.Length; m++)
                {
                    var modifier = modifiers[m];
                    AttributeSystem.UpdateAttributeModifiers(modifier.Attribute, modifier.Modifier, out _);
                }
            }
        }

        void TickGameplayEffects()
        {
            for (var i = 0; i < this.AppliedGameplayEffects.Count; i++)
            {
                var ge = this.AppliedGameplayEffects[i].spec;

                // Can't tick instant GE
                if (ge.GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.Instant) continue;

                // Update time remaining.  Stritly, it's only really valid for durational GE, but calculating for infinite GE isn't harmful
                ge.UpdateRemainingDuration(Time.deltaTime);

                // Tick the periodic component
                ge.TickPeriodic(Time.deltaTime, out var executePeriodicTick);
                if (executePeriodicTick)
                {
                    ApplyInstantGameplayEffect(ge);
                }
            }
        }

        void CleanGameplayEffects()
        {
            // this.AppliedGameplayEffects.RemoveAll(x => x.spec.GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.HasDuration && x.spec.DurationRemaining <= 0);
            
            for (int i = AppliedGameplayEffects.Count - 1; i >= 0; i--)
            {
                var effect = AppliedGameplayEffects[i];
                if (effect.spec.GameplayEffect.gameplayEffect.DurationPolicy == EDurationPolicy.HasDuration &&
                    effect.spec.DurationRemaining <= 0)
                {
                    AppliedGameplayEffects.RemoveAt(i);
                    foreach (var tag in effect.spec.GameplayEffect.gameplayEffectTags.GrantedTags)
                    {
                        OnGameplayTagChanged?.Invoke(tag, EGameplayTagEventType.Removed);
                        if (gameplayTagEvents.TryGetValue(tag, out var val))
                        {
                            InvokeTagEvent(tag, EGameplayTagEventType.Removed);
                        }
                    }
                    OnGameplayEffectRemoved?.Invoke(this, effect.spec);
                    
                }
            }
        }

        void InvokeTagEvent(GameplayTagScriptableObject tag, EGameplayTagEventType eventType)
        {
            if (gameplayTagEvents.TryGetValue(tag, out var eventList))
            {
                foreach (var eventITem in eventList)
                {
                    if (eventITem.eventType == EGameplayTagEventType.Any || eventITem.eventType == eventType)
                    {
                        eventITem.tagEvent.Invoke(tag, eventType);
                    }
                }
            }
        }
        
        protected override void OnUpdate()
        {
            // Reset all attributes to 0
            this.AttributeSystem.ResetAttributeModifiers();
            UpdateAttributeSystem();

            TickGameplayEffects();
            CleanGameplayEffects();
        }

        public void RegisterGameplayTagEvent(GameplayTagScriptableObject tag, GameplayTagEventDelegate tagEvent, EGameplayTagEventType eventType = EGameplayTagEventType.Any)
        {
            if (!gameplayTagEvents.TryGetValue(tag, out var eventList))
            {
                eventList = new List<GameplayTagEventItem>();
            }

            eventList.Add(new GameplayTagEventItem()
            {
                tagEvent = tagEvent,
                eventType = eventType
            });
        }

        public void RemoveGameplayTagEvent(GameplayTagScriptableObject tag, GameplayTagEventDelegate tagEvent)
        {
            if (gameplayTagEvents.TryGetValue(tag, out var eventList))
            {
                eventList.RemoveAll(d => d.tagEvent == tagEvent);
            }
        }
        
        public void RemoveGameplayTagEvent(GameplayTagScriptableObject tag)
        {
            if (gameplayTagEvents.TryGetValue(tag, out var val))
            {
                gameplayTagEvents.Remove(tag);
            }
        }
    }
}


namespace AbilitySystem
{
    public class GameplayEffectContainer
    {
        public GameplayEffectSpec spec;
        public ModifierContainer[] modifiers;

        public class ModifierContainer
        {
            public AttributeScriptableObject Attribute;
            public AttributeModifier Modifier;
        }
    }

    public enum EGameplayTagEventType
    {
        Any,
        Added = 1,
        Removed = 2,
    }
}