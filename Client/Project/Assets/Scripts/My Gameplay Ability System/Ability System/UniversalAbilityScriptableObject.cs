using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbilitySystem;
using AbilitySystem.Authoring;
using Battle;
using GraphProcessor;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay Ability System/Abilities/Universal")]
public class UniversalAbilityScriptableObject : AbstractAbilityScriptableObject
{
    public BaseGraph AbilityBlueprint;
    public GameplayEffectScriptableObject GameplayEffect;


    public override AbstractAbilitySpec CreateSpec(AbilitySystemCharacter owner)
    {
        var spec = new UniversalAbilitySpec(this, owner);
        spec.Level = owner.Level;
        spec.AbilityBlueprint = this.AbilityBlueprint;
        // spec.CastPointComponent = owner.GetComponent<CastPointComponent>();
        return spec;
    }


    public class UniversalAbilitySpec : AbstractAbilitySpec
    {
        private static long abilityId;
        
        public BaseGraph AbilityBlueprint;
        public CastPointComponent CastPointComponent;

        private TaskCompletionSource<bool> _taskCompletionSource;
        private SkillProcess _skillProcess;
        public long AbilityId { get; private set; }

        public UniversalAbilitySpec(AbstractAbilityScriptableObject ability, AbilitySystemCharacter owner) : base(ability, owner)
        {
            AbilityId = ++abilityId;
        }

        public override bool CanActivateAbility()
        {
            return true;
        }

        public override void CancelAbility()
        {
            return;
        }

        public override bool CheckGameplayTags()
        {
            return AscHasAllTags(Owner, this.Ability.AbilityTags.OwnerTags.RequireTags)
                    && AscHasNoneTags(Owner, this.Ability.AbilityTags.OwnerTags.IgnoreTags)
                    && AscHasAllTags(Owner, this.Ability.AbilityTags.SourceTags.RequireTags)
                    && AscHasNoneTags(Owner, this.Ability.AbilityTags.SourceTags.IgnoreTags);
        }

        protected override Task<bool> ActivateAbility()
        {
            _taskCompletionSource = new TaskCompletionSource<bool>();
            // Apply cost and cooldown
            // var cdSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cooldown);
            // var costSpec = this.Owner.MakeOutgoingSpec(this.Ability.Cost);
            // this.Owner.ApplyGameplayEffectSpecToSelf(cdSpec);
            // this.Owner.ApplyGameplayEffectSpecToSelf(costSpec);


            // Apply primary effect
            // var effectSpec = this.Owner.MakeOutgoingSpec((this.Ability as UniversalAbilityScriptableObject).GameplayEffect);
            // this.Owner.ApplyGameplayEffectSpecToSelf(effectSpec);

            _skillProcess = new SkillProcess();
            var skillUnit = new SkillUnit() { OwnerID = Owner.OwnerId };
            skillUnit.OnEndAbility = (skill) =>
            {
                Debug.Log($"{Owner.OwnerId} 技能释放完成");
                _taskCompletionSource.SetResult(true);
            };
            _skillProcess.Init(skillUnit);
            skillUnit.Skill = (Ability as UniversalAbilityScriptableObject).AbilityBlueprint;
            // _skillProcess.Run((Ability as UniversalAbilityScriptableObject).AbilityBlueprint);

            Facade.Skill.TriggerSkill(skillUnit);

            return _taskCompletionSource.Task;
            // Spawn instance of projectile prefab
        }

        protected async override Task<bool> PreActivate()
        {
            return true;
        }
    }
}

