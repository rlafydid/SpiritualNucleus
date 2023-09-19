using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public struct HeroEvadeStateData : IStateData
    {
        public Vector3 direction;
    }
    
    public class HeroEvadeState : HeroState<HeroEvadeStateData>
    {
        float t;
        Vector3 dir;

        Vector3 toPos;
        Vector3 fromPos;

        private float speed = 4f;
        
        protected override void OnEnter()
        {
            
            float angle = Vector3.SignedAngle(owner.Entity.Forward, Data.direction, Vector3.up);

            int type = (int)(angle / 45f); // 0是前面，1，2是右边，-1，-2左边, 3，4后边
            
            Debug.Log($"evada angle {angle} type {type}");
            
            switch (type)
            {
                case 0:
                    dir = owner.Entity.Forward;
                    GetActor.PlayAnim("EvadeForward");
                    break;
                case 2:
                case 1:
                    dir = owner.Entity.Right;
                    GetActor.PlayAnim("EvadeRight");
                    break;
                case -1:
                case -2:
                    dir = -owner.Entity.Right;
                    GetActor.PlayAnim("EvadeLeft");

                    break;
                default:
                    dir = -owner.Entity.Forward;
                    GetActor.PlayAnim("EvadeBack");
                    break;
            }
            t = 0;

            fromPos = owner.Position;
            toPos = fromPos + dir * 6;
        }

        protected override void OnUpdate()
        {
            t += Time.deltaTime * speed;

            Vector3 pos = Vector3.Lerp(fromPos, toPos, t);
            owner.Position = pos.ToGroundPos();
            if(t >= 1)
            {
                ExitState();
            }

        }
    }
}
