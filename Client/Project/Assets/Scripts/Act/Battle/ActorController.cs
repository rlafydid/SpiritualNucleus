using System.Collections;
using System.Collections.Generic;
using AbilitySystem;
using UnityEngine;

namespace Act.Simulator
{
    class ActorController
    {
        EntityActComponent actComponent = new EntityActComponent();

        public GameObject Entity { get; set; }

        public Vector3 Position { get => Entity.transform.position; set => Entity.transform.position = value; }

        public Quaternion Rotation { get => Entity.transform.rotation; set => Entity.transform.rotation = value; }

        public MoveComponent moveComp { get; set; }

        private AbilityController _abilityController;
        private AbilitySystemCharacter _abilitySystemCharacter;
        
        public ActInstance PlayAct(string path, List<GameObject> targets = null)
        {
            var act = actComponent.PlayAct(Entity, path, targets);
            return act;
        }

        public void StopAct()
        {
            actComponent.Destroy();
        }

        public void Update()
        {
            actComponent.Update();
            moveComp?.Update();
        }

        public void Hit()
        {
            var asset = Facade.Preview.GetHitAct();
            if (asset != null)
            {
#if UNITY_EDITOR
                string path = UnityEditor.AssetDatabase.GetAssetPath(asset);
                actComponent.PlayAct(Entity, path);
#endif
            }
        }

        public void StartMove()
        {
            moveComp = new MoveComponent();
            moveComp.owner = this;
            moveComp.speed = ConstParams.moveSpeed;
            moveComp.range = ConstParams.moveRange;
            moveComp.center = this.Position;
            moveComp.StartMove();
        }

        public void ChangeMove(Vector3 center)
        {
            moveComp.center = center;
        }

        public void StopMove()
        {
            moveComp = null;
        }

        public void Destroy()
        {
            moveComp = null;
            actComponent.Destroy();
            GameObject.Destroy(Entity);
        }
    }

    class MoveComponent
    {
        public ActorController owner;
        public float speed;
        public float range;

        public Vector3 center;

        Vector3 targetPos;

        public void StartMove()
        {
            Next();
        }

        public void Update()
        {
            Vector3 offset = targetPos - owner.Position;
            if (offset.sqrMagnitude >= 0.1f)
            {
                offset = offset.normalized * speed * Time.deltaTime;
                owner.Position += offset;
            }
            else
            {
                Next();
            }
        }

        void Next()
        {
            targetPos = new Vector3(center.x + Random.Range(-range, range), center.y, center.z + Random.Range(-range, range));
            owner.Entity.transform.LookAt(targetPos);
        }
    }

    public static class EntityExtension
    {
        public static T GetComponent<T>(this GameObject entity)
        {
            return entity.GetComponent<T>();
        }
    }
}