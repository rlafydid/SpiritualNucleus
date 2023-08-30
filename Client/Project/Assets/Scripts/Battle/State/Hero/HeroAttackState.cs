﻿using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using UnityEngine;

namespace Battle
{
    public struct AttackStateData : IStateData
    {
        public long abilityId;
    }
    public class HeroAttackState : HeroState<AttackStateData>
    {
        public override async void Enter()
        {
            owner.StopMove();

            var ability = owner.GetComponent<HeroSkillComponent>().GetAbility(Data.abilityId);
            await ability.TryActivateAbility();
            this.ToDefaultState();
        }
    }

}
