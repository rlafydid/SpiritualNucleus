using System;
using System.Collections;
using System.Collections.Generic;

namespace LKEngine
{
    public class ComponentGroup
    {
        List<Type> types = new List<Type>();
        public List<Type> GetTypes { get => types; }

        public ComponentGroup(params Type[] types)
        {
            this.types.AddRange(types);
        }

        public ComponentGroup Append(params Type[] types)
        {
            this.types.AddRange(types);
            return this;
        }
    }

}
