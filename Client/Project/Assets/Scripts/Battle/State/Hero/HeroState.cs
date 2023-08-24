using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class HeroState : HeroState<StateEmptyData>
    {
   
    }
    
    public class HeroState<T> : BaseState<T> where T : IStateData
    {
        public HeroActorController GetActor
        {
            get => owner as HeroActorController;
        }
    }
}