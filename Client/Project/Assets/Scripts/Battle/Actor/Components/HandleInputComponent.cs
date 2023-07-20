using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class HandleInputComponent : ActorComponent
    {
        public bool Jump()
        {
            return IsTrigger(KeyCode.Space);
        }

        public bool FlashMove()
        {
            return IsTrigger(KeyCode.E);
        }

        public bool Attack()
        {
            return IsTrigger(KeyCode.J);
        }

        bool IsTrigger(KeyCode code)
        {
            return Input.GetKeyDown(code);
        }
    }
}


