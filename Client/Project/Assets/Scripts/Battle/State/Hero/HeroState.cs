using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class HeroState : BaseState
    {
        public HeroActorController GetActor
        {
            get => owner as HeroActorController;
        }
    }
}