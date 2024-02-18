using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameplayTag.Authoring
{
    [CreateAssetMenu(menuName = "Gameplay Ability System/Tags")]
    public class GameplayTags : ScriptableObject
    {
        [SerializeField] public List<GameplayTagScriptableObject> tags;

        static GameplayTags _instance;

        public static void Load()
        {
            _instance = Facade.Asset.Load<GameplayTags>("GameplayTags.asset");
        }

        public static GameplayTagScriptableObject GetTag(string name)
        {
            return _instance.tags.Find(d => d.name == name);
        }
    }
}