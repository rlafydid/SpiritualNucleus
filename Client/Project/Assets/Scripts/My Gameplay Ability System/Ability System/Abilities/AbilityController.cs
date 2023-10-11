using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;
using UnityEngine.UI;

public class AbilityController : MonoBehaviour
{
    public AbstractAbilityScriptableObject[] NormalAbilities;
    public AbstractAbilityScriptableObject[] Abilities;

    public AbstractAbilityScriptableObject[] InitialisationAbilities;
    private AbilitySystemCharacter abilitySystemCharacter;

    private AbstractAbilitySpec[] abilitySpecs;
    private AbstractAbilitySpec[] normalAbilitySpecs;

    public Image[] Cooldowns;

    // Start is called before the first frame update
    void Start()
    {
        this.abilitySystemCharacter = GetComponent<AbilitySystemCharacter>();
        if (Abilities.Length > 0)
        {
            var spec = Abilities[0].CreateSpec(this.abilitySystemCharacter);
            this.abilitySystemCharacter.GrantAbility(spec);
        }
        
        ActivateInitialisationAbilities();
        GrantCastableAbilities();
    }

    // Update is called once per frame
    void Update()
    {
        for (var i = 0; i < Cooldowns.Length; i++)
        {
            var durationRemaining = this.abilitySpecs[i].CheckCooldown();
            if (durationRemaining.TotalDuration > 0)
            {
                var percentRemaining = durationRemaining.TimeRemaining / durationRemaining.TotalDuration;
                Cooldowns[i].fillAmount = 1 - percentRemaining;
            }
            else
            {
                Cooldowns[i].fillAmount = 1;
            }
        }
    }

    async void ActivateInitialisationAbilities()
    {
        for (var i = 0; i < InitialisationAbilities.Length; i++)
        {
            var spec = InitialisationAbilities[i].CreateSpec(this.abilitySystemCharacter);
            this.abilitySystemCharacter.GrantAbility(spec);
            await spec.TryActivateAbility();
        }
    }

    void GrantCastableAbilities()
    {
        this.abilitySpecs = new AbstractAbilitySpec[Abilities.Length];

        for (var i = 0; i < Abilities.Length; i++)
        {
            var spec = Abilities[i].CreateSpec(this.abilitySystemCharacter);
            this.abilitySystemCharacter.GrantAbility(spec);
            this.abilitySpecs[i] = spec;
        }

        this.normalAbilitySpecs = new AbstractAbilitySpec[NormalAbilities.Length];
        for (var i = 0; i < NormalAbilities.Length; i++)
        {
            var spec = NormalAbilities[i].CreateSpec(this.abilitySystemCharacter);
            this.abilitySystemCharacter.GrantAbility(spec);
            this.normalAbilitySpecs[i] = spec;
        }
    }

    public Task<bool> UseAbility(int i)
    {
        var spec = abilitySpecs[i];
        return spec.TryActivateAbility();
    }

    public Task<bool> UseNormalAbility(int i)
    {
        Debug.Log($"普通攻击 {i}");
        var spec = normalAbilitySpecs[i];
        return spec.TryActivateAbility();
    }

    public AbstractAbilitySpec GetAbility(int index)
    {
        return abilitySpecs[index];
    }

    public AbstractAbilitySpec GetNormalAbility(int index)
    {
        return normalAbilitySpecs[index];
    }

    public AbstractAbilitySpec GetAbilityById(long id)
    {
        foreach (var ability in abilitySpecs)
        {
            if (((UniversalAbilityScriptableObject.UniversalAbilitySpec)ability).AbilityId == id)
            {
                return ability;
            }
        }
        foreach (var ability in normalAbilitySpecs)
        {
            if (((UniversalAbilityScriptableObject.UniversalAbilitySpec)ability).AbilityId == id)
            {
                return ability;
            }
        }

        return null;
    }
}
