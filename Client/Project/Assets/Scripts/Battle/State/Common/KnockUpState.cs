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

        public override void Enter()
        {
            base.Enter();
            owner.StopMove();
            switch(Data.state)
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
                case EKnockflyAnimState.KnockBack:
                    owner.PlayAnim("KnockBack");
                    break;
            }
            t = 0;
            lastH = 0;
            speed = Data.speed;
            float radian = Data.angle * Mathf.Deg2Rad;
            if (Data.angle == 0)
            {
                v0 = Data.f;
                v = 0;
            }
            else
            {
                v0 = Mathf.Sin(radian) * Data.f;
                v = Mathf.Cos(radian) * Data.f;
            }
            
            Debug.Log($"KnockFlyState");
        }

        float lastH = 0;
        public override void Update()
        {
            base.Update();
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
                if(!owner.IsDead())
                    ChangeState(ERoleState.Idle);
            }

        }
    }
}
