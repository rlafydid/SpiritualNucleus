using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act
{
    [ActDisplayName("位移"), Serializable]
    public class ActDisplacementClip : ActBaseClip
    {
        [ActDisplayName("方向欧拉角")]
        public Vector3 eulerAngles;
        [ActDisplayName("距离")]
        public float distance;

        public override bool SupportResize => true;

        Vector3 startPos;
        Vector3 targetPos;
        
        public override bool OnTrigger()
        {
            startPos = owner.Position;
            targetPos = startPos + owner.Rotation * (Quaternion.Euler(eulerAngles) * Vector3.forward * distance);
            return false;
        }

        public override void Update(float time, float deltaTime)
        {
            base.Update(time, deltaTime);
            if (owner == null)
                return;

            float t = time / Duration;
            owner.Position  = Vector3.Lerp(startPos, targetPos, t);
        }
    }
}

