using System.Collections;
using System.Collections.Generic;
using GameplayTag.Authoring;
using UnityEngine;

public class GameplayAbilitySystem : BaseModule
{
    public override void Start()
    {
        base.Start();
        GameplayTags.Load();
    }
}
