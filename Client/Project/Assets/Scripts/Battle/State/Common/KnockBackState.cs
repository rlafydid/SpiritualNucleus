using System;
using UnityEngine;

namespace Battle
{
    public class KnockBackState : MonsterState
    {
        KnockBackData data;

        float v0;
        float a;

        float t;
        float speed;

        public override void SetParameters(object parameters)
        {
            data = (KnockBackData)parameters;
        }

        public override void Enter()
        {
            base.Enter();
            owner.StopMove();
            switch(data.state)
            {
                case EKnockflyAnimState.Hurt:
                    owner.PlayAnim("Hurt");
                    break;
                case EKnockflyAnimState.KnockFly:
                    owner.PlayAnim("Knockfly");
                    break;
                case EKnockflyAnimState.KnockFly2:
                    owner.PlayAnim("Knockfly2");
                    break;
            }
            t = 0;
            lastD = 0;
            speed = data.speed;
            v0 = data.f;
            a = data.a;
        }

        float lastD = 0;
        public override void Exexute()
        {
            base.Exexute();
            t += Time.deltaTime * speed;
            float d = v0 * t - 0.5f * a * t * t;
            float delta = d - lastD;
            lastD = d;

            Vector3 newPos = GetActor.Position + data.direction * delta;
            GetActor.Position = newPos;

            if(delta <= 0)
            {
                var pos = GetActor.Position;
                pos.y = newPos.ToGroundPos().y;
                GetActor.Position = pos;
                ChangeState(ERoleState.Idle);
            }

        }
    }
}
