using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Facade
{
    public class Command
    {
        public static Action<long, string> PlayAct;
        public static Action<long, EAction> Action;
    }
}

public enum EAction
{
    PlayAct,
    KnockUp
}

namespace Battle
{
    public class CommandModule : BaseBattleModule
    {
        public override void Init()
        {
            base.Init();
            Facade.Command.PlayAct = PlayAct;
        }

        void PlayAct(long id, string resName)
        {
            var actor = Facade.Battle.GetActor(id);
            if (actor != null)
                actor.PlayAct(resName);
        }
    }

}
