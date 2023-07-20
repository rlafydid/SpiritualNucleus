using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act.Simulator
{
    class ActSimulator
    {
        protected ActInstance actInstance;
        public ActBattleSimulator owner { get; set; }
        public float timer { get; set; }
        protected float actTimer { get => actInstance.GetTimer; }

        public virtual bool canSimulate { get; }

        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void UpdateByTime(float time) { }
        public virtual void Destroy() { }

        public virtual void OnGUI() { }
        public virtual void OnDrawGizmos() { }

        public virtual void StartPlayingAct(ActInstance actInstance)
        {
            this.actInstance = actInstance;
        }

        protected ActorController GetMainCharactor()
        {
            return owner.GetMainCharactor();
        }

        protected List<ActorController> GetTargets()
        {
            return owner.GetTargets();
        }
    }

}
