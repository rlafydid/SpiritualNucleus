using System;
using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

namespace FSM
{
    public enum EEvent
    {
        Hurt, //受伤
        KnockBack, //击退
        KnockFly, //击飞
        Vertigo, //眩晕
        Frozen,
    }
    public class FiniteStateMachine : ActorComponent
    {
        protected Dictionary<int, BaseState> states = new Dictionary<int, BaseState>();

        protected Dictionary<int, List<Transition>> events = new Dictionary<int, List<Transition>>();

        protected BaseState curState;

        public int CurrentState { get; private set; }
        public int LastState { get; private set; }

        private int _defaultState;
        
        protected override void OnStart()
        {
            ChangeState(ERoleState.Idle);
        }

        public void AddState(Enum stateType, BaseState state)
        {
            int key = stateType.ToInt();
            state.state = key;
            state.owner = ownerActor;
            state.fsm = this;
            states.Add(key, state);
        }

        public void SetDefaultState(Enum stateType)
        {
            _defaultState = stateType.ToInt();
        }

        public void ToDefaultState()
        {
            ChangeState(_defaultState);
        }

        public bool TriggerEvent(Enum stateType, IStateData data = null)
        {
            return ChangeState(stateType, data);
        }

        public bool ChangeState(Enum stateType, IStateData data = null)
        {
            int state = stateType.ToInt();
            return ChangeState(state, data);
        }
        
        /// <summary>
        /// 退出状态
        /// </summary>
        public void ExitState(BaseState state)
        {
            if (state != null)
            {
                state.Exit();
            }
            
        }

        public bool ChangeState(int state, IStateData data = null)
        {
            if (state == CurrentState)
                return false;
            
            if (states.TryGetValue(state, out BaseState target))
            {
                LastState = CurrentState;
                CurrentState = state;

                if (curState != null)
                {
                    curState.Exit();
                }

                if(ownerActor is HeroActorController)
                    Debug.Log($"英雄 转换状态为 => {(ERoleState)state}");
                else
                {
                    Debug.Log($"怪物 转换状态为 => {(ERoleState)state}");
                }

                curState = target;
                curState.SetData(data);
                curState.Enter();

                return true;
            }

            return false;
        }

        
        protected override void OnUpdate()
        {
            if (curState != null)
            {
                curState.Update();
                // if(curState.TryTransitionState(out int state))
                // {
                //     ChangeState(state);
                // }
            }

        }

        public void TriggerEvent(EEvent fsmEvent, IStateData data = null)
        {
            if(events.TryGetValue((int)fsmEvent, out var list))
            {
                foreach(var transition in list)
                {
                    if(transition.CanTransition() && (transition.fromState == -1 || transition.fromState == curState.state))
                    {
                        ChangeState(transition.toState, data);
                    }
                }
            }
        }

        public void AddEvent(EEvent fsmEvent, Transition transition)
        {
            if(!events.TryGetValue((int)fsmEvent, out var list))
            {
                list = new List<Transition>();
                events.Add((int)fsmEvent, list);
            }
            list.Add(transition);
        }
    }

    public static class StateExtensions
    {
        public static void AddState(this SceneActorController actor, Enum stateType, BaseState state)
        {
            actor.GetComponent<FiniteStateMachine>().AddState(stateType, state);
        }

        public static bool ChangeState(this SceneActorController actor, Enum stateType, IStateData param = null)
        {
            return actor.GetComponent<FiniteStateMachine>().ChangeState(stateType, param);
        }

        public static void TriggerEvent(this SceneActorController actor, FSM.EEvent fsmEvent, IStateData data = null)
        {
            actor.GetComponent<FiniteStateMachine>().TriggerEvent(fsmEvent, data);
        }

        public static void ToDefaultState(this SceneActorController actor)
        {
            actor.GetComponent<FiniteStateMachine>().ToDefaultState();
        }
        
        public static int GetCurrentState(this SceneActorController actor)
        {
            return actor.GetComponent<FiniteStateMachine>().CurrentState;
        }
        public static int ToInt(this Enum enumVal)
        {
            return (int)Enum.Parse(enumVal.GetType(), enumVal.ToString());
        }

    }
}
