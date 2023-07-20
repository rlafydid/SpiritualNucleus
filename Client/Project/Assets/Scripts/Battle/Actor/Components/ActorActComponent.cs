using System.Collections;
using System.Collections.Generic;
using Act;
using UnityEngine;

namespace Battle
{
    public class ActorActComponent : ActorComponent
    {
        EntityActComponent actComp = new EntityActComponent();

        public void PlayAct(string name)
        {
            if (name.IndexOf(".asset") < 0)
                name += ".asset";

            actComp.PlayAct(ownerActor.Entity.GameObject, name);
        }

        protected override void OnUpdate()
        {
            actComp.Update();
        }

        public void StopAct()
        {
            actComp.Destroy();
        }
    }

    public static class ActorActComponentExtensions
    {
        public static void PlayAct(this SceneActorController actor, string name)
        {
            actor.GetComponent<ActorActComponent>().PlayAct(name);
        }

        public static void StopAct(this SceneActorController actor)
        {
            actor.GetComponent<ActorActComponent>().StopAct();
        }
    }

}
