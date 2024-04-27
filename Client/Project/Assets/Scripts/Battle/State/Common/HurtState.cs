using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class HurtState : BaseState
    {
        float length = 1f;

        float t = 0;
        protected override void OnEnter()
        {
            owner.PlayAnim("Hurt");
            Debug.Log($"HurtState");
        }
        protected override void OnUpdate()
        {
            t += Time.deltaTime;
            if (t < length)
                ExitState();
        }
        protected override void OnExit()
        {
        }
    }

}
