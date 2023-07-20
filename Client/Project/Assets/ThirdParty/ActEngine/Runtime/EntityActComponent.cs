/*================================
 * 类名称: Act组件
 * 类描述: 跟角色一一对应的组件, 角色播放Act必须用此组件.
 * 目的: 
 * 创建人: Loong
 * 创建时间:
 * 修改人:
 * 修改时间:
 * 版本: @version 1.0
==================================*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Act
{
    public class EntityActComponent
    {
        List<ActInstance> instances = new List<ActInstance>();

        public ActInstance PlayAct(GameObject entity, string actName, List<GameObject> target = null)
        {
            return PlayAct(entity.ToActEntity(), actName, target?.Select(d => d.ToActEntity()).ToList());
        }

        public ActInstance PlayAct(ActEntity entity, string actName, ActEntity target = null)
        {
            List<ActEntity> targets = target != null ? new List<ActEntity>() { target } : null;
            return PlayAct(entity, actName, targets);
        }

        public ActInstance PlayAct(ActEntity entity, string actName, List<ActEntity> target = null)
        {
            int index = instances.FindIndex(d => d.ActName == actName);

            if (index >= 0)
            {
                instances[index].Destroy();
                instances.RemoveAt(index);
            }

            ActInstance instance = new ActInstance(actName);

            instances.Add(instance);

            instance.TargetList = target;

            instance.Load();
            instance.Start(entity);
            return instance;
        }

        public void Update()
        {
            for (int i = instances.Count - 1; i >= 0; i--)
            {
                var instance = instances[i];
                instance.Update();

                if (instance.LifeOver)
                {
                    instance.Destroy();
                    instances.RemoveAt(i);
                }
            }
        }

        public void ChangeSpeed(float speed)
        {
            for (int i = instances.Count - 1; i >= 0; i--)
            {
                var instance = instances[i];
                instance.PlaySpeed = speed;
            }
        }

        public void Destroy()
        {
            for (int i = instances.Count - 1; i >= 0; i--)
            {
                var instance = instances[i];
                instance.Destroy();
            }
            instances.Clear();
        }
    }
}
