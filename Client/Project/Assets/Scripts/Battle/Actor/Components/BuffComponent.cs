using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public enum EBuffEffectType
    {
        Vertigo, //眩晕
        Freeze, //冰冻
    }

    public class BuffData
    {
        public long Id;
        public int Type; //0 BUFF 1 DEBUFF
        public int EffectId;
        public EBuffEffectType EffectType;
        public long Duration;
    }

    public class BuffEffect
    {
        public long Id;
        
    }

    public class BuffUnit
    {
        public BuffData data;
        public long timer;

        public bool isFinished;

        public void Update(long delta)
        {
            timer += delta;
            if(timer >= data.Duration)
            {
                isFinished = true;
            }
        }
    }

    public class BuffEffectUnit
    {

    }

    public class BuffVertigoEffectUnit : BuffEffectUnit
    {

    }

    public class BuffComponent : ActorComponent
    {
        public List<BuffUnit> buffUnits = new List<BuffUnit>();
        public void AddBuff(BuffData data)
        {
            BuffUnit unit = new BuffUnit
            {
                data = data
            };
            buffUnits.Add(unit);
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            for(int i = buffUnits.Count-1; i >= 0; i--)
            {
                var unit = buffUnits[i];
                unit.Update((int)(Time.deltaTime * 1000));
                if (unit.isFinished)
                    buffUnits.RemoveAt(i);
            }
        }

        public void RemoveBuff()
        {

        }
    }
}
