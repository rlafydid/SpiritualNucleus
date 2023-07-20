using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class HeroFlashStateState : HeroState
    {
        float speed = 20;
        public override void Enter()
        {
            owner.PlayAnim("Charge");
            var moveComponent = GetActor.Entity.GetComponent<MoveComponent>();
            moveComponent.Finish = Finish;

        }
        public override void Exexute()
        {
            owner.Position += owner.Entity.Forward * Time.deltaTime * speed;
            if (Input.GetKeyUp(KeyCode.E))
            {
                Finish();
            }
        }

        void Finish()
        {
            ChangeState(ERoleState.Idle);
        }
    }
}