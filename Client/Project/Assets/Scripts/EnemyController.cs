using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using AbilitySystem.Authoring;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private AbstractAbilityScriptableObject initialStats;

    [SerializeField]
    private AbilitySystemCharacter asc;
    async void Start()
    {
        var spec = initialStats.CreateSpec(asc);
        asc.GrantAbility(spec);
        await spec.TryActivateAbility();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
