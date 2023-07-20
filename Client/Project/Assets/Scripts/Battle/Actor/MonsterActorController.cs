using System.Collections;
using System.Collections.Generic;
using LKEngine;
using UnityEngine;

namespace Battle
{
    public class MonsterActorController : SceneActorController
    {
        protected override ComponentGroup GetEntityComponents => base.GetEntityComponents.Append(typeof(ColliderComponent));

        public override void OnInit()
        {
            base.OnInit();
            AddComponent<NormalAttacksComponent>();
            InitState();
        }

        void InitState()
        {
            var fsm = GetComponent<FSM.FiniteStateMachine>();

            MonsterIdleState idleState = new MonsterIdleState();
            MonsterTraceTargetState traceState = new MonsterTraceTargetState();
            MonsterAttackState attackState = new MonsterAttackState();
            KnockFlyState knockFlyState = new KnockFlyState();
            KnockBackState knockBackState = new KnockBackState();
            HurtState hurtState = new HurtState();
            MonsterDeadState deadState = new MonsterDeadState();

            fsm.AddState(ERoleState.Idle, idleState);
            fsm.AddState(ERoleState.TraceTarget, traceState);
            fsm.AddState(ERoleState.Attack, attackState);
            fsm.AddState(ERoleState.KnockFly, knockFlyState);
            fsm.AddState(ERoleState.Hurt, hurtState);
            fsm.AddState(ERoleState.Dead, deadState);
            fsm.AddState(ERoleState.KnockBack, knockBackState);

            var toHurtTransition = new FSM.Transition(ERoleState.Hurt);
            fsm.AddEvent(FSM.EEvent.Hurt, toHurtTransition);

            var toKnockBackTransition = new FSM.Transition(ERoleState.KnockBack);
            fsm.AddEvent(FSM.EEvent.KnockBack, toKnockBackTransition);

            var toKnockFlyTransition = new FSM.Transition(ERoleState.KnockFly);
            fsm.AddEvent(FSM.EEvent.KnockFly, toKnockFlyTransition);

            var toVertigoTransition = new FSM.Transition(ERoleState.Vertigo);
            fsm.AddEvent(FSM.EEvent.Vertigo, toVertigoTransition);
        }

        public override void Start()
        {
            base.Start();
            this.Entity.LookAt(this.Position - this.Entity.Forward);
            this.RandomAttack();
        }

        public void TraceTarget(Entity entity)
        {
            moveComponent.TraceTarget(entity);
        }
    }
}

