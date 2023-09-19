using System;
using UnityEngine;

namespace Battle
{
    public class KnockFlyState : MonsterState<KnockFlyData>
    {
        float v0;
        float g = 10;

        float v;

        float t;
        float speed;

        protected override void OnEnter()
        {
            if (Data == null)
            {
                ExitState();
                return;
            }
            owner.StopMove();

            owner.PlayAnim("Knockfly");
            owner.DontToDefaultAnimation();
            t = 0;
            lastH = 0;
            speed = Data.speed;
            float radian = Data.angle * Mathf.Deg2Rad;
            if (Data.angle == 0)
            {
                v0 = 0;
                v = Data.f;
            }
            else
            {
                v0 = Mathf.Sin(radian) * Data.f;
                v = Mathf.Cos(radian) * Data.f;
            }
            
            Debug.Log($"KnockFlyState");
        }

        float lastH = 0;
        protected override void OnUpdate()
        {
            t += Time.deltaTime * speed;
            float h = v0 * t - 0.5f * g * t * t;
            float deltaH = h - lastH;
            lastH = h;

            Vector3 newPos = GetActor.Position + Vector3.up * deltaH + Data.direction * v * Time.deltaTime * speed;
            GetActor.Position = newPos;

            if(owner.IsDead())
                owner.Entity.GameObject.GetComponent<SimpleAnimation>().ReturnToDefaultState = false;
            
            if(newPos.y < newPos.ToGroundPos().y)
            {
                var pos = GetActor.Position;
                pos.y = newPos.ToGroundPos().y;
                GetActor.Position = pos;
                IsUpdate = false;
                TimerMod.Delay(owner.IsDead() ? 0 : 2, () =>
                {
                    owner.TurnOnToDefaultAnimation();
                    ExitState();
                });
            }

        }
    }
}
