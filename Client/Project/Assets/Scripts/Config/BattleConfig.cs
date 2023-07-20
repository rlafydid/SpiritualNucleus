using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelConfig
{
    public long Id;
    public string ResName;

    static List<ModelConfig> configList = new List<ModelConfig>()
    {
        { new ModelConfig(){ Id = 1001, ResName = "baixiaofei02.prefab"}},
        { new ModelConfig(){ Id = 1002, ResName = "bingshe01.prefab"}},
        { new ModelConfig(){ Id = 1003, ResName = "assassin.prefab"}},
        { new ModelConfig(){ Id = 2001, ResName = "monster1.prefab"}},
        { new ModelConfig(){ Id = 2002, ResName = "sunxiaohou01.prefab"}},
    };


    static Dictionary<long, ModelConfig> _configs = null;
    static Dictionary<long, ModelConfig> configs
    {
        get
        {
            if (_configs == null)
            {
                _configs = new Dictionary<long, ModelConfig>();
                foreach (var item in configList)
                {
                    _configs.Add(item.Id, item);
                }
            }
            return _configs;
        }
    }

    public static ModelConfig Get(long id)
    {
        if (configs.TryGetValue(id, out ModelConfig config))
        {
            return config;
        }
        else
        {
            throw new System.Exception($"在模型表中没找到{id}对应的模型资源");
        }
    }
}

public class CharacterConfig
{
    public long Id;
    public long ModelConfigId;

    public long Hp;
    public long Attack;

    public string[] NormalAttacks;
    public string Skill1;
    public string Skill2;
    public string Skill3;

    static List<CharacterConfig> configList = new List<CharacterConfig>()
    {
        { new CharacterConfig(){ Id = 1001, ModelConfigId = 1001,
            Hp = 1000,
            Attack = 5,
            NormalAttacks =  new string[] {"BaiXiaoFei_Attack1.asset" ,  "BaiXiaoFei_Attack2.asset",  "BaiXiaoFei_Attack3.asset",  "BaiXiaoFei_Attack4.asset"},
            Skill1 = "BaiXiaoFei_Skill1.asset",
            Skill2 = "BaiXiaoFei_Skill2.asset",
            Skill3 = "BaiXiaoFei_Skill3.asset",
        }},
        { new CharacterConfig(){ Id = 1002, ModelConfigId = 1002}},
        { new CharacterConfig(){ Id = 1003, ModelConfigId = 1003,
            Attack = 5,
            NormalAttacks =  new string[] {"BP_Assassin_Attack1.asset",  "BP_Assassin_Attack2.asset", "BP_Assassin_Attack3.asset"},
            Skill1 = "BP_Assassin_Skill1.asset",
        }
        },
        { new CharacterConfig(){ Id = 2001, ModelConfigId = 2001}},
        { new CharacterConfig(){ Id = 2002, ModelConfigId = 2002,
            Hp = 100,
            Attack = 5,
            NormalAttacks =  new string[] {"SunXiaoHou_Attack1.asset", "SunXiaoHou_Attack2.asset", "SunXiaoHou_Attack3.asset", "SunXiaoHou_Attack4.asset" },
        }

        },
    };


    static Dictionary<long, CharacterConfig> _configs = null;
    static Dictionary<long, CharacterConfig> configs
    {
        get
        {
            if (_configs == null)
            {
                _configs = new Dictionary<long, CharacterConfig>();
                foreach (var item in configList)
                {
                    _configs.Add(item.Id, item);
                }
            }
            return _configs;
        }
    }

    public static CharacterConfig Get(long id)
    {
        if (configs.TryGetValue(id, out CharacterConfig config))
        {
            return config;
        }
        else
        {
            throw new System.Exception($"在模型表中没找到{id}对应的模型资源");
        }
    }
}

public class SceneConfig
{

}
