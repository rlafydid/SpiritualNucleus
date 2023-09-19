using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

namespace Battle
{
    public class HeroActorController : SceneActorController
    {
        public override void OnInit()
        {
            base.OnInit();
            AddComponent<HeroSkillComponent>();
            AddComponent<JoystickMoveComponent>();
            AddComponent<HandleInputComponent>();
            AddComponent<OperateComponent>();
            InitState();
            
            
        }

        void InitState()
        {
            var fsm = GetComponent<FSM.FiniteStateMachine>();
            HeroIdleState idleState = new HeroIdleState();
            HeroMoveState moveState = new HeroMoveState();
            HeroAttackState attackState = new HeroAttackState();
            HeroJumpState jumpState = new HeroJumpState();
            HeroFlashState flashState = new HeroFlashState();
            HeroEvadeState evadeState = new HeroEvadeState();

            fsm.AddState(ERoleState.Idle, idleState);
            fsm.AddState(ERoleState.Move, moveState);
            fsm.AddState(ERoleState.Attack, attackState);
            fsm.AddState(ERoleState.Jump, jumpState);
            fsm.AddState(ERoleState.FlashMove, flashState);
            fsm.AddState(ERoleState.Evade, evadeState);

            var handleInput = this.GetComponent<HandleInputComponent>();

            FSM.Transition toIdle = new FSM.Transition(ERoleState.Idle);
            
            FSM.Transition toMove = new FSM.Transition(ERoleState.Move);
            // toMove.AddCondition(GetComponent<JoystickMoveComponent>().IsMoving);

            FSM.Transition toAttack = new FSM.Transition(ERoleState.Attack);
            // toAttack.AddCondition(this.GetComponent<HeroSkillComponent>().FindSkill);

            FSM.Transition toJump = new FSM.Transition(ERoleState.Jump);
            // toJump.AddCondition(handleInput.Jump);

            FSM.Transition toFlashMove = new FSM.Transition(ERoleState.FlashMove);
            // toFlashMove.AddCondition(handleInput.FlashMove);

            FSM.Transition toEvade = new FSM.Transition(ERoleState.Evade);
            
            idleState.AddTransition(toMove);
            idleState.AddTransition(toAttack);
            idleState.AddTransition(toJump);
            idleState.AddTransition(toFlashMove);
            idleState.AddTransition(toEvade);

            moveState.AddTransition(toAttack);
            moveState.AddTransition(toJump);
            moveState.AddTransition(toFlashMove);
            moveState.AddTransition(toEvade);
            
            evadeState.AddTransition(toIdle);
        }

        public void TriggerEvent(ERoleState state)
        {
            GetComponent<FSM.FiniteStateMachine>().TriggerEvent(state);
        }
        

        public override void Start()
        {
            base.Start();
            //moveComponent.SetDefaultState(EMoveState.Joystick);
        }

        public void StartMove()
        {
            GetComponent<JoystickMoveComponent>().Active = true;
            IsMove = true;
            // Entity.GetComponent<AnimationController>().PlayDefault();
        }

        public override void StopMove()
        {
            base.StopMove();
            // Entity.GetComponent<AnimationController>().PlayDefault();
            GetComponent<JoystickMoveComponent>().Active = false;
        }

        public override void Update()
        {
            base.Update();
        }
        
    }

    
}
