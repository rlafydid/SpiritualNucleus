using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class HeroEvadeState : HeroState
    {
        EEvadeDirection direction;
        float v0;
        float g = 10;

        float v;

        float t;
        float speed = 3;

        float lastH = 0;
        float angle = 40;
        float f = 10;

        Vector3 dir;

        Vector3 toPos;
        Vector3 fromPos;

        public override void Enter()
        {
            base.Enter();
            direction = owner.GetComponent<OperateComponent>().evadeDirection;
            switch (direction)
            {
                case EEvadeDirection.Forward:
                    dir = owner.Entity.Forward;
                    GetActor.PlayAnim("EvadeForward");
                    break;
                case EEvadeDirection.Back:
                    dir = -owner.Entity.Forward;
                    GetActor.PlayAnim("EvadeBack");

                    break;
                case EEvadeDirection.Left:
                    dir = -owner.Entity.Right;
                    GetActor.PlayAnim("EvadeLeft");

                    break;
                case EEvadeDirection.Right:
                    dir = owner.Entity.Right;
                    GetActor.PlayAnim("EvadeRight");
                    break;
            }
            t = 0;
            lastH = 0;
            float radian = angle * Mathf.Deg2Rad;

            v0 = Mathf.Sin(radian) * f;
            v = Mathf.Cos(radian) * f;

            fromPos = owner.Position;
            toPos = fromPos + dir * 6;
        }

        public override void Update()
        {
            t += Time.deltaTime * speed;
            //float h = v0 * t - 0.5f * g * t * t;
            //float deltaH = h - lastH;
            //lastH = h;

            //Vector3 newPos = GetActor.Position + Vector3.up * deltaH + dir * v * Time.deltaTime * speed;
            //GetActor.Position = newPos;

            //if (newPos.y < newPos.ToGroundPos().y)
            //{
            //    Debug.Log("结束躲避");
            //    var pos = GetActor.Position;
            //    pos.y = newPos.ToGroundPos().y;
            //    GetActor.Position = pos;
            //    ChangeState(EState.Idle);
            //}

            Vector3 pos = Vector3.Lerp(fromPos, toPos, t);
            owner.Position = pos.ToGroundPos();
            if(t >= 1)
            {
                ChangeState(ERoleState.Idle);
            }

        }
    }
}
