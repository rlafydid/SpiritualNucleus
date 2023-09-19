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

        public override void Enter()
        {
            base.Enter();
            if (Data == null)
            {
                ExitState();
                return;
            }
                
            owner.StopMove();
            // switch(Data.state)
            // {
            //     case EKnockflyAnimState.Hurt:
            //         owner.PlayAnim("Hurt");
            //         break;
            //     case EKnockflyAnimState.KnockFly:
            //         owner.PlayAnim("Knockfly");
            //         break;
            //     case EKnockflyAnimState.KnockFly2:
            //         owner.PlayAnim("Knockfly2");
            //         break;
            // }

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
            
            Debug.Log($"KnockBackState");
        }


        public override void Update()
        {
            base.Update();

            t += Time.deltaTime;
            
            float v = v0 + -a * t;
            float yV = -g * t * 0.5f;

            Vector3 offset = Data.direction * v;
            offset.y = velocity.y * Time.deltaTime;
            
            velocity.y -= g * Time.deltaTime * 2;
            
            Debug.Log($"v0:{v} height:{yV} velocity:{velocity}");
            
            Vector3 newPos = GetActor.Position + offset;

          
            
            GetActor.Position = newPos;

            if(v <= 0 && newPos.y <= newPos.ToGroundPos().y)
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
