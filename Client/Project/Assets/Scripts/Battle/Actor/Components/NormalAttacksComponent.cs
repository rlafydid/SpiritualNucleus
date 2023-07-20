using System.Collections;
using System.Collections.Generic;
using Battle;
using UnityEngine;

namespace Battle
{
    public class NormalAttacksComponent : ActorComponent
    {
        public List<string> normalAttacks = new List<string>();

        public float attackInterval = 3;
        int normalAttackIndex = 0;
        float lastAttackTime = 0;

        bool isRandom = false;

        protected override void OnStart()
        {
            normalAttacks.Clear();
            SetNormalAttacks(ownerActor.CharacterConfig.NormalAttacks);
        }

        public void SetNormalAttacks(string[] attacks)
        {
            normalAttacks = new List<string>(attacks);
        }

        public void Random()
        {
            isRandom = true;
        }

        public SkillUnit FindRandomSkill()
        {
            int index = UnityEngine.Random.Range(0, normalAttacks.Count);
            string res = normalAttacks[index];

            return new SkillUnit() { BPRes = res, OwnerID = ownerActor.ID };
        }

        public SkillUnit FindSkill()
        {
            if (Time.realtimeSinceStartup - lastAttackTime > attackInterval || normalAttackIndex >= normalAttacks.Count)
            {
                normalAttackIndex = 0;
            }
            lastAttackTime = Time.realtimeSinceStartup;
            string res = normalAttacks[normalAttackIndex++];
            return new SkillUnit() { BPRes = res, OwnerID = ownerActor.ID };
        }
    }

    public static class NormalAttacksComponentExtensions
    {
        public static void RandomAttack(this SceneActorController actor)
        {
            actor.GetComponent<NormalAttacksComponent>().Random();
        }

    }
}
