using System.Collections;
using System.Collections.Generic;
using LKEngine;

public class BattleScene : BaseScene
{
    public override void OnEnter()
    {
        base.OnEnter();
        Facade.Battle.StartBattle();
    }
}
