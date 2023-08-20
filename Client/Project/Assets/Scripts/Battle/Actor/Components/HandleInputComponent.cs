using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Battle
{
    public class HandleInputComponent : ActorComponent, DefaultInputActions.IHeroActions
    {
        private DefaultInputActions playerInput;
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
            this.ownerActor.GetComponent<JoystickMoveComponent>().SetMoveDelta(new Vector3(delta.x, 0, delta.y));
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            this.ownerActor.GetComponent<HeroSkillComponent>().UseNormalAbility();
            ChangeToState(ERoleState.Attack);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            ChangeToState(ERoleState.Jump);
        }

        public void OnSkill1(InputAction.CallbackContext context)
        {
            this.ownerActor.GetComponent<HeroSkillComponent>().UseAbility(0);
            ChangeToState(ERoleState.Attack);
        }

        public void OnSkill2(InputAction.CallbackContext context)
        {
            this.ownerActor.GetComponent<HeroSkillComponent>().UseAbility(1);
            ChangeToState(ERoleState.Attack);
        }

        public void OnSkill3(InputAction.CallbackContext context)
        {
            this.ownerActor.GetComponent<HeroSkillComponent>().UseAbility(2);
            ChangeToState(ERoleState.Attack);
        }

        public void OnSkill4(InputAction.CallbackContext context)
        {
            this.ownerActor.GetComponent<HeroSkillComponent>().UseAbility(3);
            ChangeToState(ERoleState.Attack);
        }

        public void OnDodge(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        void ChangeToState(ERoleState state)
        {
            var fsm = this.ownerActor.GetComponent<FSM.FiniteStateMachine>();
            fsm.ChangeState(state);
        }
    }
}


