using System;
using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using FSM;
using LKEngine;
using UnityEngine;


namespace Battle
{
    public class ActorComponent : LKEngine.Component
    {
        public SceneActorController GetActor { get => ownerActor; }
        protected SceneActorController ownerActor;

        public void Setup(SceneActorController owner)
        {
            this.ownerActor = owner;
        }
    }

    public class SceneActorController
    {
        public Entity Entity;
        
        public long ID { get; set; }

        protected int configID;
        protected string resName;

        public Vector3 Position { get => Entity.Position; set => Entity.Position = value; }

        static long staticID;

        ModelConfig _modelConfig;
        public ModelConfig ModelConfig { get => _modelConfig; }

        CharacterConfig _characterConfig;
        public CharacterConfig CharacterConfig { get => _characterConfig; }

        Dictionary<Type, ActorComponent> actorComponents = new Dictionary<Type, ActorComponent>();

        public bool IsMove { get => moveComponent.IsMove; set => moveComponent.IsMove = value; }

        protected AnimationController animationCtrl;
        protected MoveComponent moveComponent;

        protected virtual ComponentGroup GetEntityComponents { get; } = new ComponentGroup(typeof(MoveComponent), typeof(AnimationController));

        public float MoveSpeed
        {
            get
            {
                return moveComponent.MoveSpeed;
            }

            set
            {
                moveComponent.MoveSpeed = value;
            }
        }

        public void SetConfig(int id)
        {
            this.configID = id;
            _characterConfig = CharacterConfig.Get(id);
            _modelConfig = ModelConfig.Get(_characterConfig.ModelConfigId);
        }

        public virtual void OnInit()
        {
            this.AddComponent<FSM.FiniteStateMachine>();
            this.AddComponent<ActorActComponent>();
            this.AddComponent<AttributesComponent>();
        }

        public void Load()
        {
            Entity = SceneManager.Instance.AddEntity(_modelConfig.ResName, GetEntityComponents);
            this.ID = Entity.Id;
            animationCtrl = Entity.GetComponent<AnimationController>();
            moveComponent = Entity.GetComponent<MoveComponent>();
            MoveSpeed = 2;
            IsMove = true;
            Entity.Finished = Start;
            Entity.Load();
        }

        public virtual void Start()
        {
            foreach (var comp in actorComponents)
            {
                comp.Value.Start();
            }
        }

        public virtual void Update()
        {
            foreach(var comp in actorComponents)
            {
                comp.Value.Update();
            }
        }

        public virtual void LateUpdate()
        {
            Vector3 groundPos = Position.ToGroundPos();
            if (Position.y < groundPos.y)
            {
                Position = groundPos;
            }
            foreach (var comp in actorComponents)
            {
                comp.Value.LateUpdate();
            }
        }

        public T AddComponent<T>() where T : ActorComponent, new()
        {
            T comp = new T();
            actorComponents.Add(typeof(T), comp);
            comp.Setup(this);
            comp.Awake();
            return comp;
        }

        public T GetComponent<T>() where T : ActorComponent
        {
            actorComponents.TryGetValue(typeof(T), out var comp);
            return (T)comp;
        }

        public void Hurt(DamageData data)
        {
            switch (data)
            {
                case KnockFlyData knockUpData:
                    this.TriggerEvent(FSM.EEvent.KnockFly, knockUpData);
                    break;
                case KnockBackData knockBackData:
                    this.TriggerEvent(FSM.EEvent.KnockBack, knockBackData);
                    break;
                default:
                    this.TriggerEvent(FSM.EEvent.Hurt, data);
                    break;
            }
            this.LoseHp(data.value);
            var obj = ResourceManager.Instance.Instantiate<GameObject>("DamageNumber.prefab");
            obj.transform.position = this.Position + Vector3.up * 0.5f;
            obj.GetComponentInChildren<DamageNumberMesh>().number = data.value;
            AudioManager.Instance.PlayAudio("baixiaofei01_attack_liuxingyu_hit.wav");
        }

        public void PlayAnim(string name, bool returnToDefaultState = false)
        {
            animationCtrl.CrossFade(name, 0.3f, returnToDefaultState);
        }

        public virtual void StopMove()
        {
            moveComponent.Stop();
        }

        public void Destroy()
        {
            SceneManager.Instance.RemoveEntity(Entity);
        }

        public void StopAttack()
        {
            this.StopAct();
            Facade.Skill.StopSkill(ID);
        }
    }

    public class DamageData
    {
        public long value;
    }

    public class KnockFlyData : DamageData
    {
        public float angle; //角度
        public float f; //力
        public Vector3 direction; //方向
        public float speed;
        public EKnockflyAnimState state;
    }

    public class KnockBackData : DamageData
    {
        public float f; //力
        public float a; //减速度
        public Vector3 direction; //方向
        public float speed;
        public EKnockflyAnimState state;
    }
}