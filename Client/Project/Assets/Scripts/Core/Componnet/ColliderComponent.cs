using System.Collections;
using System.Collections.Generic;
using Collision;
using UnityEngine;
using LKEngine;
using System;

public class ColliderComponent : LKEngine.Component
{
    public Action<Entity> onTriggerEnter;
    public Action<Entity> onTriggerExit;

    BaseCollider collider;
    protected override void OnStart()
    {
        collider = new BaseCollider(Owner as Entity);
        collider.onTriggerEnter = OnTriggerEnter;
        collider.onTriggerExit = OnTriggerExit;
        Facade.World.Add(collider);
    }

    void OnTriggerEnter(Entity entity)
    {
        onTriggerEnter?.Invoke(entity);
    }

    void OnTriggerExit(Entity entity)
    {
        onTriggerExit?.Invoke(entity);
    }

    protected override void OnDestroy()
    {
        Facade.World.Remove(collider);
    }
}
