using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public struct HeroFlashStateData : IStateData
    {
        public Vector3 direction;
    }
    public class HeroFlashState : HeroState<HeroFlashStateData>
    {
        float speed = 20;
        protected override void OnEnter()
        {
            owner.PlayAnim("Charge");
            var moveComponent = GetActor.Entity.GetComponent<MoveComponent>();
            moveComponent.MoveLerpTo(owner.Position + Data.direction * 10, 1);
            moveComponent.Finish = Finish;

        }
        protected override void OnUpdate()
        {
        }

        void Finish()
        {
            MakeStateTransitionable();
        }
    }
}