using System.Collections;
using System.Collections.Generic;
using FSM;
using UnityEngine;

namespace Battle
{
    public class AttributesComponent : ActorComponent
    {
        public long hp;
        public long attack;

        protected override void OnAwake()
        {
            hp = ownerActor.CharacterConfig.Hp;
            attack = ownerActor.CharacterConfig.Attack;
        }

        public void LoseHp(long val)
        {
            hp -= val;
            // if (IsDead())
            // {
            //     this.GetActor.ChangeState(ERoleState.Dead);
            // }
        }

        public bool IsDead()
        {
            return hp <= 0;
        }
    }

    public static class AttributesExtensions
    {
        public static void LoseHp(this SceneActorController actor, long val)
        {
            actor.GetComponent<AttributesComponent>().LoseHp(val);
        }

        public static bool IsDead(this SceneActorController actor)
        {
            return actor.GetComponent<AttributesComponent>().IsDead();
        }
    }
}

