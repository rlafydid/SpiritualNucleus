using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public enum EEvadeDirection
    {
        Left,
        Right,
        Forward,
        Back
    }

    public class EvadeCondition : FSM.Condition
    {
        float lastTime;
        KeyCode lastKeyCode;
        const float evadeTime = 1f;
        Dictionary<KeyCode, EEvadeDirection> keys = new Dictionary<KeyCode, EEvadeDirection>()
            {
            { KeyCode.A, EEvadeDirection.Left },
            { KeyCode.D, EEvadeDirection.Right },
            { KeyCode.S, EEvadeDirection.Back },
            { KeyCode.W, EEvadeDirection.Forward },
            };

        public override bool Pass()
        {
            foreach (var item in keys)
            {
                if (Input.GetKeyDown(item.Key))
                {
                    if (Time.realtimeSinceStartup - lastTime < evadeTime && lastKeyCode == item.Key)
                    {
                        owner.GetComponent<OperateComponent>().evadeDirection = item.Value;
                        return true;
                    }
                    lastTime = Time.realtimeSinceStartup;
                    lastKeyCode = item.Key;
                }
            }
            return false;
        }
    }
}

