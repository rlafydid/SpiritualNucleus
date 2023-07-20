using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillConfig
{
    public int ConfigId;
    public long Move;
    public string ActName;

    static Dictionary<int, SkillConfig> confTable = new Dictionary<int, SkillConfig>();

    public static SkillConfig Get(int id)
    {
         if(confTable.TryGetValue(id, out SkillConfig conf))
        {
            return conf;
        }
        return null;
    }
}


