using System;

namespace Battle
{
    public class MonsterState : BaseState
    {
        public MonsterActorController GetActor
        {
            get => owner as MonsterActorController;
        }
    }
}

