using System;

namespace Battle
{
    
    public class MonsterState : MonsterState<StateEmptyData>
    {
   
    }
    
    public class MonsterState<T> : BaseState<T> where T : IStateData
    {
        public MonsterActorController GetActor
        {
            get => owner as MonsterActorController;
        }
    }
}

