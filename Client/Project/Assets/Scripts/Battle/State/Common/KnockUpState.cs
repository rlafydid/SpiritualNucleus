using System;
using UnityEngine;

namespace Battle
{
    public class KnockFlyState : MonsterState
    {
        KnockFlyData data;

        float v0;
        float g = 10;

        float v;

        float t;
        float speed;

        public override void SetParameters(object parameters)
        {
            data = (KnockFlyData)parameters;
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
            lastH = 0;
            speed = data.speed;
            float radian = data.angle * Mathf.Deg2Rad;
            if (data.angle == 0)
            {
                v0 = data.f;
                v = 0;
            }
            else
            {
                v0 = Mathf.Sin(radian) * data.f;
                v = Mathf.Cos(radian) * data.f;
            }
        }

        float lastH = 0;
        public override void Exexute()
        {
            base.Exexute();
            t += Time.deltaTime * speed;
            float h = v0 * t - 0.5f * g * t * t;
            float deltaH = h - lastH;
            lastH = h;

            Vector3 newPos = GetActor.Position + Vector3.up * deltaH + data.direction * v * Time.deltaTime * speed;
            GetActor.Position = newPos;

            if(newPos.y < newPos.ToGroundPos().y)
            {
                var pos = GetActor.Position;
                pos.y = newPos.ToGroundPos().y;
                GetActor.Position = pos;
                ChangeState(ERoleState.Idle);
            }

        }
    }
}
