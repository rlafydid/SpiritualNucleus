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

        public void OnMove(InputAction.CallbackContext context)
        {
            
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            this.GetComponent<HeroSkillComponent>().SetupSkill(1);
            ChangeToState(ERoleState.Attack);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            ChangeToState(ERoleState.Jump);
        }

        public void OnSkill1(InputAction.CallbackContext context)
        {
            this.GetComponent<HeroSkillComponent>().SetupSkill(1);
            ChangeToState(ERoleState.Attack);
        }

        public void OnSkill2(InputAction.CallbackContext context)
        {
            this.GetComponent<HeroSkillComponent>().SetupSkill(2);
            ChangeToState(ERoleState.Attack);
        }

        public void OnSkill3(InputAction.CallbackContext context)
        {
            this.GetComponent<HeroSkillComponent>().SetupSkill(3);
            ChangeToState(ERoleState.Attack);
        }

        public void OnSkill4(InputAction.CallbackContext context)
        {
            this.GetComponent<HeroSkillComponent>().SetupSkill(4);
            ChangeToState(ERoleState.Attack);
        }

        public void OnDodge(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        void ChangeToState(ERoleState state)
        {
            var fsm = GetComponent<FSM.FiniteStateMachine>();
            fsm.ChangeState(state);
        }
    }
}


