using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public struct BLocation
    {
        public Vector3 position;
        public Quaternion rotation;

        public BLocation(Vector3 pos) : this(pos, default)
        {
        }
        public BLocation(Vector3 pos, Quaternion rot)
        {
            position = pos;
            rotation = rot;
        }

        public static implicit operator BLocation(Vector3 pos)
        {
            return new BLocation(pos);
        }
    }

    public struct BCalculateItem
    {
        public BLocation self;
        public BLocation target;
        public float speed;
        public float deltaTime;
    }

    public class BulletMath
    {
        public static BLocation TickForTrace(BCalculateItem item, bool isTurnToTargetNow)
        {
            BLocation result = item.self;

            Vector3 dire = (item.target.position - item.self.position).normalized;
            Quaternion wantQuat = Quaternion.LookRotation(dire);
            if (wantQuat != item.self.rotation)
            {
                if (isTurnToTargetNow)
                {
                    result.rotation = wantQuat;
                }
                else
                {
                    //float time = targetDis / moveSpeed;
                    //float turnNeedTime = Math.Max(time * BulletConst.BULLET_TRACK_ROTATION_PARAM2.AsFloat(), 0.001f);
                    //float rate = deltaTime / turnNeedTime;
                    //Quaternion curQuat = Quaternion.Slerp(actor.Rot, wantQuat, rate);
                    result.rotation = wantQuat;
                }
            }
            result.position = TickForLine(item);
            return result;
        }

        public static Vector3 TickForLine(BCalculateItem item)
        {
            float dis = item.speed * item.deltaTime;
            item.self.position += item.self.rotation * Vector3.forward * dis;
            return item.self.position;
        }

        public static BLocation TickForMissile(BCalculateItem item, Vector3 direction, float upAcceleration, ref float gravity)
        {
            BLocation result = item.self;
            //Vector3 targetPos = bulletCom.targerPos.ToVector3();
            Vector3 pos = item.self.position;
            if (item.self.position.y >= -1f)//targetPos.y
            {
                float dis = item.speed * item.deltaTime;
                gravity -= upAcceleration * item.deltaTime;
                Vector3 newPos = pos + direction * dis + new Vector3(0, gravity * item.deltaTime, 0);
                Vector3 dir = Vector3.Normalize(newPos - pos);
                result.rotation = Quaternion.LookRotation(dir);
                result.position = newPos;
            }
            return result;
        }
    }
}
