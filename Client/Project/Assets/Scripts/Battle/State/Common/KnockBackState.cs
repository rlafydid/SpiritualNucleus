using System;
using UnityEngine;

namespace Battle
{
    public class KnockBackState : MonsterState<KnockBackData>
    {
        float v0;
        float a;

        float t;
        float speed;
        
        float g = 10;
        private Vector3 velocity;

        private bool isUpdate = false;

        protected override void OnEnter()
        {
            if (Data == null)
            {
                ExitState();
                return;
            }
                
            owner.StopMove();

            if (owner.IsFloatingState())
            {
                ChangeState(ERoleState.KnockFly,new KnockFlyData(){ f = Data.f, angle = 0, direction = Data.direction, speed = Data.speed});
                return;
            }
            else
                owner.PlayAnim("Hurt");
            
            t = 0;
            speed = Data.speed;
            v0 = Data.f;
            a = Data.a;
            velocity = Vector3.zero;

            isUpdate = true;
            Debug.Log($"KnockBackState");
        }


        protected override void OnUpdate()
        {
            if (!isUpdate)
                return;
            
            t += Time.deltaTime;
            
            float v = v0 + -a * t;

            Vector3 offset = Data.direction * v;
            offset.y = 0;
            Debug.Log($"v0:{v}");
            
            Vector3 newPos = GetActor.Position + offset;

            GetActor.Position = newPos;

            if(v <= 0)
            {
                var pos = GetActor.Position;
                pos.y = newPos.ToGroundPos().y;
                GetActor.Position = pos;
                owner.TurnOnToDefaultAnimation();
                ExitState();
            }

        }
    }
}
