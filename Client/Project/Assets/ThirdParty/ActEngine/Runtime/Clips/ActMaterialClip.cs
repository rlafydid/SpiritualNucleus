using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Act
{
    public enum EMaterialEffectType
    {
        Frozen
    }
    [ActDisplayName("材质", "材质效果"), Serializable]
    public class ActMaterialClip : ActBaseClip
    {
        [ActDisplayName("效果")]
        public EMaterialEffectType effect;
        //public float speed = 1;

        public override bool SupportResize => false;
        
        private Material mat;

        private Dictionary<Renderer, Material> _materials;

        public override bool OnTrigger()
        {
            var renderers = (owner as GameObjectEntity).GameObject.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                _materials.Add(renderer, renderer.material);

                Facade.Externals.LoadAssetAsync("Frozen", (obj) =>
                {
                    renderer.material = (obj as Material);
                });
            }
            return true;
        }

        public override void Update(float time, float deltaTime)
        {
            
        }

        public override void OnDisable()
        {
            base.OnDisable();
            foreach (var item in _materials)
            {
                item.Key.material = item.Value;
            }
        }
    }
}
