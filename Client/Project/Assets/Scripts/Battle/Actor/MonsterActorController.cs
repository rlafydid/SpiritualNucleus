using System.Collections;
using System.Collections.Generic;
using FSM;
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
            // AddComponent<NormalAttacksComponent>();
            AddComponent<HeroSkillComponent>();
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

            idleState.AddTransition(new Transition(ERoleState.Hurt));
            idleState.AddTransition(new Transition(ERoleState.KnockBack));
            idleState.AddTransition(new Transition(ERoleState.KnockFly));
            idleState.AddTransition(new Transition(ERoleState.TraceTarget));
            idleState.AddTransition(new Transition(ERoleState.Attack));
            
            traceState.AddTransition(new Transition(ERoleState.Hurt));
            traceState.AddTransition(new Transition(ERoleState.KnockBack));
            traceState.AddTransition(new Transition(ERoleState.KnockFly));
            traceState.AddTransition(new Transition(ERoleState.Attack));

            var toIdleTransition = new FSM.Transition(ERoleState.Idle);
            toIdleTransition.priority = -1;
            
            var toHurtTransition = new FSM.Transition(ERoleState.Hurt);
            fsm.AddEvent(FSM.EEvent.Hurt, toHurtTransition);

            var toKnockBackTransition = new FSM.Transition(ERoleState.KnockBack);
            fsm.AddEvent(FSM.EEvent.KnockBack, toKnockBackTransition);

            var toKnockFlyTransition = new FSM.Transition(ERoleState.KnockFly);
            fsm.AddEvent(FSM.EEvent.KnockFly, toKnockFlyTransition);
            toKnockFlyTransition.priority = -2;

            var toVertigoTransition = new FSM.Transition(ERoleState.Vertigo);
            fsm.AddEvent(FSM.EEvent.Vertigo, toVertigoTransition);

            var toDeadTranstion = new FSM.Transition(ERoleState.Dead);
            toDeadTranstion.priority = 10;
            
            hurtState.AddTransition(toHurtTransition);
            hurtState.AddTransition(toKnockBackTransition);
            
            // knockBackState.AddTransition(toKnockBackTransition);
            knockBackState.AddTransition(toKnockFlyTransition);
            knockBackState.AddTransition(toIdleTransition);
            
            knockFlyState.AddTransition(toKnockFlyTransition);
            knockFlyState.AddTransition(toKnockBackTransition);
            knockFlyState.AddTransition(toHurtTransition);
            knockFlyState.AddTransition(toIdleTransition);
            
            attackState.AddTransition(toHurtTransition);
            attackState.AddTransition(toKnockBackTransition);
            attackState.AddTransition(toKnockFlyTransition);

            toDeadTranstion.AddCondition(new AttributeCondition(){ AttributeValue = 0, AttributeType = "hp", CompareType = ECompareType.LEqual});
            
            knockBackState.AddTransition(toDeadTranstion);
            knockFlyState.AddTransition(toDeadTranstion);
            hurtState.AddTransition(toDeadTranstion);
        }

        public override void Start()
        {
            base.Start();
            this.Entity.LookAt(this.Position - this.Entity.Forward);
            // this.RandomAttack();
        }

        public void TraceTarget(Entity entity)
        {
            moveComponent.TraceTarget(entity);
        }
    }
}

