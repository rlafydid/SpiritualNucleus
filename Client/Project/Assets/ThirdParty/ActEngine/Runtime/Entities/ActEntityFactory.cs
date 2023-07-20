using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Act
{
    public static class ActEntityFactory
    {
        public static ActEntity ToActEntity(this GameObject entity)
        {
            if (entity == null)
                return null;
            return new GameObjectEntity(entity);
        }

        public static ActEntity Instantiate(GameObject prefab)
        {
            GameObject obj = GameObject.Instantiate(prefab);
            return obj.ToActEntity();
        }

        public static ActEntity CreateEntity(string assetName)
        {
            GameObjectEntity entity = new GameObjectEntity();
            entity.ResName = assetName;
            return entity;
        }
    }

}


