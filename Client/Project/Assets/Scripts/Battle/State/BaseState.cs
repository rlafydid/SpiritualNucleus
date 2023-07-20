using System;
using System.Collections;
using System.Collections.Generic;
using FSM;
using UnityEngine;

namespace Battle
{
    public interface IState
    {
        void Enter();
        void Exexute();
        void Exit();
    }

    public class TransitionParams
    {
        public int lastState;
        public object param;
    }

    public class BaseState : IState
    {
        public SceneActorController owner;
        public FiniteStateMachine fsm;

        protected List<Transition> transitions = new List<Transition>();

        public int state;

        public virtual void SetParameters(object parameters)
        {
        }
        public virtual void Enter()
        {
        }
        public virtual void Exexute() { }
        public virtual void Exit() { }

        public void AddTransition(Transition transition)
        {
            transition.fsm = fsm;
            transitions.Add(transition);
        }

        public bool TryTransitionState(out int state)
        {
            foreach(var trasition in transitions)
            {
                if(trasition.CanTransition())
                {
                    state = trasition.toState;
                    return true;
                }
            }
            state = 0;
            return false;
        }

        protected void ChangeState(Enum state, TransitionParams param = null)
        {
            fsm.ChangeState(state, param);
        }

        protected void TriggerEvent(Enum state, object param = null)
        {
            fsm.TriggerEvent(state, param);
        }
    }
}
