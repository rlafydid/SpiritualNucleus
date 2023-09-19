using System.Collections;
using System.Collections.Generic;
using FSM;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Battle
{
    public class HandleInputComponent : ActorComponent, DefaultInputActions.IHeroActions
    {
        public static DefaultInputActions playerInput;
        protected override void OnStart()
        {
            base.OnStart();
            playerInput = new DefaultInputActions();
            playerInput.Enable();
            playerInput.Hero.SetCallbacks(this);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            var delta = context.ReadValue<Vector2>();
            this.ownerActor.GetComponent<JoystickMoveComponent>().SetMoveDir(new Vector3(delta.x, 0, delta.y));
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            Debug.Log($"OnAttack");
            this.ownerActor.GetComponent<HeroSkillComponent>().UseNormalAbility();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            ChangeToState(ERoleState.Jump);
        }

        public void OnSkill1(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            this.ownerActor.GetComponent<HeroSkillComponent>().UseAbility(0);
        }

        public void OnSkill2(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            this.ownerActor.GetComponent<HeroSkillComponent>().UseAbility(1);
        }

        public void OnSkill3(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            this.ownerActor.GetComponent<HeroSkillComponent>().UseAbility(2);
        }

        public void OnSkill4(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            this.ownerActor.GetComponent<HeroSkillComponent>().UseAbility(3);
        }

        public void OnDodge(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            Vector3 dir = this.ownerActor.GetComponent<JoystickMoveComponent>().GetMoveDirection;
            if (dir.magnitude < 0.001f)
            {
                dir = ownerActor.Entity.Forward;
            }
            ownerActor.ChangeState(ERoleState.Evade, new HeroEvadeStateData(){ direction = dir});
        }

        void ChangeToState(ERoleState state)
        {
            var fsm = this.ownerActor.GetComponent<FSM.FiniteStateMachine>();
            fsm.ChangeState(state);
        }
    }
}


