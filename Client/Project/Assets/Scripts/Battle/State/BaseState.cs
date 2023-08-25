using System;
using System.Collections;
using System.Collections.Generic;
using FSM;
using UnityEngine;

namespace Battle
{
    public interface IState
    {
        void SetData(IStateData data);
        void Enter();
        void Update();
        void Exit();
    }

    public interface IStateData
    {
        
    }

    public struct StateEmptyData : IStateData
    {
    }

    public class TransitionParams
    {
        public int lastState;
        public object param;
    }

    public class BaseState<T> : BaseState where T : IStateData
    {
        protected T Data { get; private set; }

        public override void SetData(IStateData data)
        {
            if(data != null)
                Data = (T)data;
        }
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

        public virtual void SetData(IStateData data)
        {
        }
        
        public virtual void Enter()
        {
        }
        public virtual void Update() { }
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
        
        /// <summary>
        /// 是否可以转换到指定状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool CanTransitionToState(int state)
        {
            if (transitions.Count == 0)
                return true;
            
            foreach (var trasition in transitions)
            {
                if (state == trasition.toState && trasition.CanTransition())
                {
                    return true;
                }
            }

            return false;
        }

        protected void ChangeState(Enum state, IStateData param = null)
        {
            fsm.ChangeState(state, param);
        }

        protected void TriggerEvent(Enum state, IStateData param = null)
        {
            fsm.TriggerEvent(state, param);
        }
    }
}
