using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class HeroJumpState : HeroState
    {
        float v0;
        float g = 10;

        float v;

        float t;
        float speed = 2;

        float lastH = 0;
        float f = 10;

        int jumpCount = 0;

        public override void Enter()
        {
            base.Enter();

            t = 0;
            lastH = 0;

            v0 = f;
            jumpCount = 0;
            owner.PlayAnim("Jump");
        }

        void ContinueJump()
        {
            t = 0;
            lastH = 0;

            if (jumpCount == 0)
                owner.PlayAnim("Jump2");
            else
                owner.PlayAnim("Jump3");
            jumpCount++;
        }

        public override void Update()
        {
            t += Time.deltaTime * speed;
            float h = v0 * t - 0.5f * g * t * t;
            float deltaH = h - lastH;
            lastH = h;

            Vector3 newPos = GetActor.Position + Vector3.up * deltaH;
            GetActor.Position = newPos;

            if (newPos.y < newPos.ToGroundPos().y)
            {
                var pos = GetActor.Position;
                pos.y = newPos.ToGroundPos().y;
                GetActor.Position = pos;
                ChangeState(ERoleState.Idle);
            }

            // if(jumpCount < 2 && owner.GetComponent<HandleInputComponent>().Jump())
            // {
            //     ContinueJump();
            // }
        }
    }
}
