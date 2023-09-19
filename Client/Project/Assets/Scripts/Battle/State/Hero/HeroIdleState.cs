using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class HeroIdleState : HeroState
    {
        protected override void OnEnter()
        {
            //GetActor.PlayAct("idle");
            GetActor.StartMove();
           
        }
        protected override void OnUpdate()
        {
        }
    }
}