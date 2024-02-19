using System.Collections;
using System.Collections.Generic;
using Battle;
using GraphProcessor;
using UnityEngine;

[NodeMenuItem("发射器/创建发射器")]
public class LauncherNode : UniversalNodeWithOut
{
    public override string name => "发射器";
    [Input("发射器资源"), SerializeField]
    public string res;

    protected override void Process()
    {
        base.Process();
        var unit = new SkillUnit()
        {
            BPRes = res,
            OwnerID = skilUnit.OwnerID,
        };
        Facade.Skill.TriggerLauncher(unit);
    }
}
