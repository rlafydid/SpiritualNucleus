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
            }
            t = 0;
            speed = Data.speed;
            v0 = Data.f;
            a = Data.a;
            
            Debug.Log($"KnockBackState");
        }

        private Vector3 velocity;

        private float flyV;
        
        public override void Update()
        {
            base.Update();

            t += Time.deltaTime;
            
            float v = v0 + -a * t;
            float yV = -g * t;

            velocity = Data.direction * v;
            velocity.y = yV;
            
            Debug.Log($"v0:{v} height:{yV} velocity:{velocity}");
            
            Vector3 newPos = GetActor.Position + velocity;

            if (yV < newPos.ToGroundPos().y)
            {
                newPos.y = newPos.ToGroundPos().y;
            }
            
            GetActor.Position = newPos;

            if (owner.IsDead())
                owner.Entity.GameObject.GetComponent<SimpleAnimation>().ReturnToDefaultState = false;
            
            if(v <= 0 && newPos.y <= newPos.ToGroundPos().y)
            {
                var pos = GetActor.Position;
                pos.y = newPos.ToGroundPos().y;
                GetActor.Position = pos;
                if (owner.IsDead())
                {
                    TimerMod.Delay(2f, () =>
                    {
                        Facade.Battle.ReleaseActor(owner.ID);
                    });
                }
                else
                {
                    ChangeState(ERoleState.Idle);
                }
                
            }

        }
    }
}
