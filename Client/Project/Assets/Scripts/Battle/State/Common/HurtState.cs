using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class HurtState : BaseState
    {
        float length = 1f;

        float t = 0;
        public override void Enter()
        {
            base.Enter();
            owner.PlayAnim("Hurt");
            Debug.Log($"HurtState");
        }
        public override void Update()
        {
            base.Update();
            t += Time.deltaTime;
            if (t < length)
                ChangeState(ERoleState.Idle);
        }
        public override void Exit()
        {
            base.Exit();
        }
    }

}
