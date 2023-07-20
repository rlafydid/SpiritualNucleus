using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class HeroIdleState : HeroState
    {
        public override void Enter()
        {
            //GetActor.PlayAct("idle");
            GetActor.StartMove();
           
        }
        public override void Exexute()
        {
        }
    }
}